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

            });

            //  Post /invoices/{id}/match
            app.MapPost("/invoices/{id}/match", async (
                int id,
                AgentApiDbContext db
            ) =>
            {

            });
        }
    }
}