namespace P2P.AgentApi.Records
{
    public sealed record CreatePurchaseOrderRequest(
        int VendorId,
        IReadOnlyList<CreatePurchaseOrderLineRequest> Lines
    );
}