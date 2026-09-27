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

            });
        }
    }
}