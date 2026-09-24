using Microsoft.EntityFrameworkCore;
using P2P.AgentApi.Data;
using P2P.AgentApi.Entities;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Endpoints
{
    public static class GeneralLedgerEndpoints
    {
        public static void MapGeneralLedgerEndpoints(
            this WebApplication app
        )
        {
            //  Post /invoices/{id}/approve
            app.MapPost("/invoices/{id}/approve", async (
                int id,
                AgentApiDbContext db
            ) =>
            {
                var invoice = await db.Invoices
                    .Include(invoice => invoice.Vendor)
                    .SingleOrDefaultAsync(invoice => invoice.Id == id);

                if (invoice is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Invoice not found",
                        detail: $"Invoice {id} was not found.");
                }

                if (invoice.Status != InvoiceStatus.MATCHED)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: "Invoice is not matched",
                        detail: "Only matched invoices can be approved and posted to the general ledger.");
                }

                var expenseAccountCode = invoice?.Vendor?.Category?
                    .ToString()
                    .ToUpperInvariant() switch
                {
                    "INVENTORY" => "EXP-INVENTORY",
                    "SERVICES" => "EXP-SERVICES",
                    "FREIGHT" => "EXP-FREIGHT",
                    "OFFICE" => "EXP-OFFICE",
                    _ => "EXP-GENERAL"
                };

                var postedAt = DateTime.UtcNow;

                invoice?.Approve();

                var ledgerEntries = new[]
                {
                    new GLEntry
                    {
                        InvoiceId = invoice!.Id,
                        AccountId = 1000,
                        AccountCode = "AP Control",
                        Debit = invoice.Amount,
                        Credit = 0m,
                        PostedAt = postedAt
                    },
                    new GLEntry
                    {
                        InvoiceId = invoice.Id,
                        AccountId = 2000,
                        AccountCode = expenseAccountCode,
                        Debit = 0m,
                        Credit = invoice.Amount,
                        PostedAt = postedAt
                    }
                };

                db.GLEntries.AddRange(ledgerEntries);

                await db.SaveChangesAsync();

                return Results.Ok(new
                {
                    invoice.Id,
                    invoice.PurchaseOrderId,
                    invoice.Amount,
                    invoice.Status,
                    LedgerEntries = ledgerEntries.Select(entry => new
                    {
                        entry.Id,
                        entry.AccountCode,
                        entry.Debit,
                        entry.Credit,
                        entry.PostedAt
                    })
                });
            });
        }
    }
}