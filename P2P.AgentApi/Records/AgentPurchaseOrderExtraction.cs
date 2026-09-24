namespace P2P.AgentApi.Records
{
    public sealed record AgentPurchaseOrderExtraction(
        string? VendorName,
        string? Sku,
        int? Quantity,
        decimal? UnitCost
    );
}