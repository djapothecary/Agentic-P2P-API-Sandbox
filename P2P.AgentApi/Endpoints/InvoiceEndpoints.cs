using Microsoft.EntityFrameworkCore;
using P2P.AgentApi.Data;
using P2P.AgentApi.Entities;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Endpoints
{
    public static class InvoiceEndpoints
    {
        public static void MapInvoiceEndpoints(
            this WebApplication app
        )
        {
            //  Post /invoices
            app.MapPost("/invoices", async (
                CreateInvoiceRequest request,
                AgentApiDbContext db
            ) =>
            {
                if (request is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid invoice request",
                        detail: "An invoice request is required.");
                }

                if (request.Amount <= 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid invoice amount",
                        detail: "Invoice amount must be greater than zero.");
                }

                var purchaseOrder = await db.PurchaseOrders
                    .Include(po => po.Vendor)
                    .SingleOrDefaultAsync(po =>
                        po.Id == request.PurchaseOrderId);

                if (purchaseOrder is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Purchase order not found",
                        detail:
                            $"Purchase order {request.PurchaseOrderId} was not found.");
                }

                var invoice = new Invoice
                {
                    VendorId = purchaseOrder.VendorId,
                    PurchaseOrderId = purchaseOrder.Id,
                    Amount = request.Amount,
                    Status = InvoiceStatus.PENDING
                };

                db.Invoices.Add(invoice);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/invoices/{invoice.Id}",
                    new
                    {
                        invoice.Id,
                        invoice.PurchaseOrderId,
                        invoice.VendorId,
                        invoice.Amount,
                        invoice.Status
                    });
            });

            //  Post /invoices/{id}/match
            app.MapPost("/invoices/{id}/match", async (
                int id,
                AgentApiDbContext db
            ) =>
            {
                var invoice = await db.Invoices
                   .Include(invoice => invoice.PurchaseOrder)
                       .ThenInclude(po => po.POLineItems)  //   this might cause a run-time warning
                   .SingleOrDefaultAsync(invoice => invoice.Id == id);

                if (invoice is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Invoice not found",
                        detail: $"Invoice {id} was not found.");
                }

                if (invoice.Status != InvoiceStatus.PENDING)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Invalid invoice state",
                        detail: "Only pending invoices can be matched.");
                }

                var purchaseOrderLines = invoice.PurchaseOrder.POLineItems;

                if (purchaseOrderLines.Count == 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Purchase order has no lines",
                        detail:
                            "The invoice cannot be matched because its purchase order has no line items.");
                }

                var purchaseOrderValue = purchaseOrderLines.Sum(line =>
                    line.QuantityOrdered * line.UnitCost);

                var receivedGoodsValue = purchaseOrderLines.Sum(line =>
                    line.QuantityReceived * line.UnitCost);

                var isPartiallyReceived = purchaseOrderLines.Any(line =>
                    line.QuantityReceived < line.QuantityOrdered);

                if (invoice.Amount > receivedGoodsValue)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Invoice exceeds received goods value",
                        detail:
                            $"Invoice amount {invoice.Amount:C} exceeds the received goods value of {receivedGoodsValue:C}.");
                }

                if (isPartiallyReceived)
                {
                    return Results.Json(
                        new
                        {
                            invoice.Id,
                            invoice.PurchaseOrderId,
                            invoice.Amount,
                            Status = "PendingPartialReceipt",
                            PurchaseOrderValue = purchaseOrderValue,
                            ReceivedGoodsValue = receivedGoodsValue,
                            RemainingGoodsValue =
                                purchaseOrderValue - receivedGoodsValue
                        },
                        statusCode: StatusCodes.Status202Accepted);
                }

                if (invoice.Amount != purchaseOrderValue ||
                    invoice.Amount != receivedGoodsValue)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Three-way match failed",
                        detail:
                            $"Invoice amount {invoice.Amount:C} must equal both the purchase order value {purchaseOrderValue:C} and the received goods value {receivedGoodsValue:C}.");
                }

                invoice.MarkMatches();
                await db.SaveChangesAsync();

                return Results.Ok(
                    new
                    {
                        invoice.Id,
                        invoice.PurchaseOrderId,
                        invoice.Amount,
                        invoice.Status,
                        PurchaseOrderValue = purchaseOrderValue,
                        ReceivedGoodsValue = receivedGoodsValue,
                        MatchStatus = "Matched"
                    });
            });
        }
    }
}