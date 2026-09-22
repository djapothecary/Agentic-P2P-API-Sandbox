namespace P2P.AgentApi.Records
{
    public sealed record CreatePurchaseOrderLineRequest(
        string Sku,
        string Description,
        int QuantityOrdered,
        decimal UnitCost
    );
}