using Microsoft.EntityFrameworkCore;
using P2P.AgentApi.Data;
using P2P.AgentApi.Entities;
using P2P.AgentApi.Enums;
using P2P.AgentApi.Records;

namespace P2P.AgentApi.Endpoints
{
    public static class PurchaseOrderEndpoints
    {
        public static void MapPurchaseOrderEndpoints(
            this WebApplication app
        )
        {
            //  Post /purchase-order
            app.MapPost("/purchase-orders", async (
                CreatePurchaseOrderRequest request,
                AgentApiDbContext db
            ) =>
            {
                if (request.Lines is null || request.Lines.Count == 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid purchase order",
                        detail: "At least one purchase-order line is required.");
                }

                if (request.Lines.Any(line =>
                        string.IsNullOrWhiteSpace(line.Sku) ||
                        line.QuantityOrdered <= 0 ||
                        line.UnitCost < 0))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid purchase-order line",
                        detail: "Each line must have an SKU, a positive quantity, and a non-negative unit cost.");
                }

                var vendor = await db.Vendors
                    .SingleOrDefaultAsync(vendor => vendor.Id == request.VendorId);

                if (vendor is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Vendor not found",
                        detail: $"Vendor {request.VendorId} was not found.");
                }

                if (!vendor.IsActive)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Inactive vendor",
                        detail: "Inactive vendors cannot have new purchase orders.");
                }

                var purchaseOrder = new PurchaseOrder(request.VendorId);

                foreach (var line in request.Lines)
                {
                    purchaseOrder.POLineItems.Add(new POLineItem(
                        line.Sku,
                        line.Description,
                        line.QuantityOrdered,
                        line.UnitCost));
                }

                db.PurchaseOrders.Add(purchaseOrder);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/purchase-orders/{purchaseOrder.Id}",
                    new
                    {
                        purchaseOrder.Id,
                        purchaseOrder.VendorId,
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

            //  Post /purchase-orders/{id}/submit
            app.MapPost("/purchase-orders/{id}/submit", async (
                int id,
                AgentApiDbContext db
            ) =>
            {
                var purchaseOrder = await db.PurchaseOrders
                .Include(po => po.POLineItems)
                .SingleOrDefaultAsync(po => po.Id == id);

                if (purchaseOrder is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Purchase order not found",
                        detail: $"Purchase order {id} was not found.");
                }

                if (purchaseOrder.Status != PurchaseOrderStatus.DRAFT)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Invalid purchase-order state",
                        detail: "Only draft purchase orders can be submitted.");
                }

                if (purchaseOrder.POLineItems.Count == 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Purchase order has no lines",
                        detail: "A purchase order must contain at least one line before it can be submitted.");
                }

                purchaseOrder.Status = PurchaseOrderStatus.SUBMITTED;
                await db.SaveChangesAsync();

                return Results.Ok(new
                {
                    purchaseOrder.Id,
                    purchaseOrder.Status
                });
            });

            //  Post /purchase-orders/{id}/receive
            app.MapPost("/purchase-orders/{id}/receive", async (
                int id,
                ReceivePurchaseOrderRequest request,
                AgentApiDbContext db
            ) =>
            {
                if (request is null ||
                string.IsNullOrWhiteSpace(request.ReceivedBy) ||
                request.Lines is null ||
                request.Lines.Count == 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid goods receipt",
                        detail: "ReceivedBy and at least one receipt line are required.");
                }

                if (request.Lines.Any(line => line.QuantityReceived <= 0))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid receipt quantity",
                        detail: "Receipt quantities must be greater than zero.");
                }

                if (request.Lines
                    .GroupBy(line => line.POLineItemId)
                    .Any(group => group.Count() > 1))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Duplicate receipt line",
                        detail: "Each purchase-order line may appear only once in a goods receipt.");
                }

                var purchaseOrder = await db.PurchaseOrders
                    .Include(po => po.POLineItems)
                    .SingleOrDefaultAsync(po => po.Id == id);

                if (purchaseOrder is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Purchase order not found",
                        detail: $"Purchase order {id} was not found.");
                }

                if (purchaseOrder.Status is not
                    (PurchaseOrderStatus.SUBMITTED and
                     PurchaseOrderStatus.RECEIVED))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Invalid purchase-order state",
                        detail: "Only submitted purchase orders can receive goods.");
                }

                var requestedLineIds = request.Lines
                    .Select(line => line.POLineItemId)
                    .ToHashSet();

                var purchaseOrderLines = purchaseOrder.POLineItems
                    .Where(line => requestedLineIds.Contains(line.Id))
                    .ToDictionary(line => line.Id);

                var missingLineId = requestedLineIds
                    .FirstOrDefault(lineId => !purchaseOrderLines.ContainsKey(lineId));

                if (missingLineId != 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid purchase-order line",
                        detail: $"Line {missingLineId} does not belong to purchase order {id}.");
                }

                foreach (var requestedLine in request.Lines)
                {
                    var purchaseOrderLine = purchaseOrderLines[requestedLine.POLineItemId];
                    var remainingQuantity =
                        purchaseOrderLine.QuantityOrdered -
                        purchaseOrderLine.QuantityReceived;

                    if (requestedLine.QuantityReceived > remainingQuantity)
                    {
                        return Results.Problem(
                            statusCode: StatusCodes.Status422UnprocessableEntity,
                            title: "Receipt quantity exceeds ordered quantity",
                            detail:
                                $"Line {purchaseOrderLine.Id} has only {remainingQuantity} remaining units available to receive.");
                    }
                }

                var receipt = new GoodsReceipt();
                var receiptEntry = db.Entry(receipt);

                receiptEntry.Property(nameof(GoodsReceipt.PurchaseOrderId))
                    .CurrentValue = purchaseOrder.Id;
                receiptEntry.Property(nameof(GoodsReceipt.ReceivedBy))
                    .CurrentValue = request.ReceivedBy;
                receiptEntry.Property(nameof(GoodsReceipt.ReceivedAt))
                    .CurrentValue = DateTime.UtcNow;

                foreach (var requestedLine in request.Lines)
                {
                    var purchaseOrderLine = purchaseOrderLines[requestedLine.POLineItemId];

                    purchaseOrderLine.QuantityReceived += requestedLine.QuantityReceived;

                    receipt.Lines.Add(new GoodsReceiptLine
                    {
                        POLineItemId = purchaseOrderLine.Id,
                        QuantityReceived = requestedLine.QuantityReceived
                    });
                }

                db.GoodsReceipts.Add(receipt);

                var fullyReceived = purchaseOrder.POLineItems
                    .All(line => line.QuantityReceived >= line.QuantityOrdered);

                purchaseOrder.Status = fullyReceived
                    ? PurchaseOrderStatus.RECEIVED
                    : PurchaseOrderStatus.SUBMITTED;

                await db.SaveChangesAsync();

                return Results.Created(
                    $"/purchase-orders/{purchaseOrder.Id}",
                    new
                    {
                        purchaseOrder.Id,
                        purchaseOrder.Status,
                        ReceiptId = receipt.Id,
                        receiptEntry.Property(nameof(GoodsReceipt.ReceivedBy)).CurrentValue,
                        Lines = receipt.Lines.Select(line => new
                        {
                            line.POLineItemId,
                            line.QuantityReceived
                        })
                    });
            });

            //  Get /purchase-orders/{id}
            app.MapGet("/purchase-orders/{id}", async (
                int id,
                AgentApiDbContext db
            ) =>
            {
                var purchaseOrder = await db.PurchaseOrders
                 .AsNoTracking()
                 .Include(po => po.Vendor)
                 .Include(po => po.POLineItems)
                 .Include(po => po.GoodsReceipts)
                     .ThenInclude(receipt => receipt.Lines)
                 .SingleOrDefaultAsync(po => po.Id == id);

                if (purchaseOrder is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Purchase order not found",
                        detail: $"Purchase order {id} was not found.");
                }

                return Results.Ok(new
                {
                    purchaseOrder.Id,
                    purchaseOrder.VendorId,
                    Vendor = new
                    {
                        purchaseOrder.Vendor.Id,
                        purchaseOrder.Vendor.Name,
                        purchaseOrder.Vendor.IsActive
                    },
                    purchaseOrder.Status,
                    purchaseOrder.CreatedAt,
                    Lines = purchaseOrder.POLineItems.Select(line => new
                    {
                        line.Id,
                        line.Sku,
                        line.Description,
                        line.QuantityOrdered,
                        line.QuantityReceived,
                        QuantityRemaining =
                            line.QuantityOrdered - line.QuantityReceived,
                        IsFullyReceived =
                            line.QuantityReceived >= line.QuantityOrdered,
                        line.UnitCost
                    }),
                    Receipts = purchaseOrder.GoodsReceipts.Select(receipt => new
                    {
                        receipt.Id,
                        receipt.ReceivedBy,
                        receipt.ReceivedAt,
                        Lines = receipt.Lines.Select(line => new
                        {
                            line.POLineItemId,
                            line.QuantityReceived
                        })
                    }),
                    ReceiptStatus = new
                    {
                        TotalOrdered = purchaseOrder.POLineItems
                            .Sum(line => line.QuantityOrdered),
                        TotalReceived = purchaseOrder.POLineItems
                            .Sum(line => line.QuantityReceived),
                        TotalRemaining = purchaseOrder.POLineItems
                            .Sum(line => line.QuantityOrdered - line.QuantityReceived),
                        IsFullyReceived = purchaseOrder.POLineItems
                            .All(line => line.QuantityReceived >= line.QuantityOrdered)
                    }
                });
            });
        }
    }
}