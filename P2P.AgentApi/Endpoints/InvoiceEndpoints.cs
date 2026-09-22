using P2P.AgentApi.Data;
using P2P.AgentApi.Entities;

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
                AgentApiDbContext db
            ) =>
            {
                //  TODO:   ReasonMCP will generate this logic during the interview
                return Results.Ok();
            });

            //  Post /invoices/{id}/match
            app.MapPost("/invoices/{id}/match", async (
                int id,
                AgentApiDbContext db
            ) =>
            {
                //  TODO:   ReasonMCP will generate this logic during the interview
                return Results.Ok();
            });
        }
    }
}