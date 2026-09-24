using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using P2P.AgentApi.Data;
using P2P.AgentApi.Entities;
using P2P.AgentApi.Records;

namespace P2P.AgentApi.Endpoints;

public static class AgentEndpoints
{
    private const string SystemPrompt = """
        You are a strict data-extraction service for a Purchase-to-Pay API.

        Extract the following values from the user's purchase-order request:
        - VendorName: the vendor or supplier name
        - Sku: the product SKU
        - Quantity: the ordered quantity as a positive integer
        - UnitCost: the per-unit price as a decimal number

        Return ONLY valid JSON. Do not return Markdown, code fences, explanations,
        comments, or additional properties.

        The response must exactly match this JSON schema:
        {
          "VendorName": "string",
          "Sku": "string",
          "Quantity": 0,
          "UnitCost": 0.00
        }

        If any required value cannot be determined, return ONLY valid JSON using
        null for the missing value:
        {
          "VendorName": null,
          "Sku": null,
          "Quantity": null,
          "UnitCost": null
        }
        """;

    public static void MapAgentEndpoints(this WebApplication app)
    {
        app.MapPost("/agent/execute", async (
            AgentExecuteRequest request,
            IHttpClientFactory httpClientFactory,
            AgentApiDbContext db,
            IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Text))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid agent request",
                    detail: "A non-empty Text value is required.");
            }

            var ollamaBaseUrl =
                configuration["Ollama:BaseUrl"] ??
                "http://localhost:11434";

            var ollamaModel =
                configuration["Ollama:Model"] ??
                "llama3.2";

            var ollamaClient = httpClientFactory.CreateClient();
            ollamaClient.BaseAddress = new Uri(ollamaBaseUrl);

            OllamaGenerateResponse ollamaResponse;

            try
            {
                using var ollamaRequest = new HttpRequestMessage(
                    HttpMethod.Post,
                    "/api/generate")
                {
                    Content = JsonContent.Create(new
                    {
                        model = ollamaModel,
                        system = SystemPrompt,
                        prompt = request.Text,
                        stream = false,
                        format = "json"
                    })
                };

                using var response = await ollamaClient.SendAsync(
                    ollamaRequest,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status502BadGateway,
                        title: "Language model request failed",
                        detail:
                            $"Ollama returned HTTP {(int)response.StatusCode}.");
                }

                ollamaResponse =
                    await response.Content.ReadFromJsonAsync
                        <OllamaGenerateResponse>(
                            cancellationToken);

                if (ollamaResponse is null ||
                    string.IsNullOrWhiteSpace(ollamaResponse.Response))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status502BadGateway,
                        title: "Empty language model response",
                        detail: "The language model did not return extracted data.");
                }
            }
            catch (HttpRequestException exception)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status502BadGateway,
                    title: "Language model unavailable",
                    detail: exception.Message);
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status408RequestTimeout,
                    title: "Language model request timed out",
                    detail: "The language model did not respond in time.");
            }

            AgentPurchaseOrderExtraction? extractedRequest;

            try
            {
                extractedRequest =
                    JsonSerializer.Deserialize
                        <AgentPurchaseOrderExtraction>(
                            ollamaResponse.Response,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
            }
            catch (JsonException exception)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status502BadGateway,
                    title: "Invalid language model JSON",
                    detail: $"The language model returned invalid JSON: {exception.Message}");
            }

            if (extractedRequest is null ||
                string.IsNullOrWhiteSpace(extractedRequest.VendorName) ||
                string.IsNullOrWhiteSpace(extractedRequest.Sku) ||
                extractedRequest.Quantity is null ||
                extractedRequest.UnitCost is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status422UnprocessableEntity,
                    title: "Incomplete purchase-order request",
                    detail:
                        "The language model could not extract VendorName, Sku, Quantity, and UnitCost.");
            }

            if (extractedRequest.Quantity <= 0 ||
                extractedRequest.UnitCost < 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status422UnprocessableEntity,
                    title: "Invalid extracted purchase-order values",
                    detail:
                        "Quantity must be greater than zero and UnitCost must not be negative.");
            }

            var vendor = await db.Vendors
                .SingleOrDefaultAsync(
                    candidate => candidate.Name.ToLower() ==
                        extractedRequest.VendorName.ToLower(),
                    cancellationToken);

            if (vendor is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Vendor not found",
                    detail:
                        $"No vendor named '{extractedRequest.VendorName}' was found.");
            }

            if (!vendor.IsActive)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status422UnprocessableEntity,
                    title: "Inactive vendor",
                    detail: "Inactive vendors cannot have new purchase orders.");
            }

            var purchaseOrder = new PurchaseOrder(vendor.Id);

            purchaseOrder.POLineItems.Add(new POLineItem(
                extractedRequest.Sku,
                $"Created from agent request: {request.Text}",
                extractedRequest.Quantity.Value,
                extractedRequest.UnitCost.Value));

            db.PurchaseOrders.Add(purchaseOrder);

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status422UnprocessableEntity,
                    title: "Purchase order could not be saved",
                    detail: exception.Message);
            }

            return Results.Created(
                $"/purchase-orders/{purchaseOrder.Id}",
                new
                {
                    purchaseOrder.Id,
                    purchaseOrder.VendorId,
                    VendorName = vendor.Name,
                    purchaseOrder.Status,
                    purchaseOrder.CreatedAt,
                    Lines = purchaseOrder.POLineItems.Select(line => new
                    {
                        line.Id,
                        line.Sku,
                        line.Description,
                        line.QuantityOrdered,
                        line.QuantityReceived,
                        line.UnitCost
                    })
                });
        });
    }
}