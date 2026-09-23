namespace P2P.AgentApi.Endpoints
{
    public static class TroubleshootingEndpoints
    {
        public static void MapRawAPIEndpoint(
            this WebApplication app
        )
        {
            //  For testing payload configurations
            app.MapPost("/troubleshooting", async (HttpContext context) =>
            {
                // PUT YOUR BREAKPOINT ON THE CONSOLE.WRITELINE BELOW
                Console.WriteLine("\n[GATEWAY] Request made it through the door!");

                // Read the raw body as a string to see exactly what TS sent
                using var reader = new StreamReader(context.Request.Body);
                var rawJson = await reader.ReadToEndAsync();

                Console.WriteLine($"[RAW JSON]: {rawJson}");

                return Results.Ok(new { response = "Raw connection successful." });
            });
        }
    }
}