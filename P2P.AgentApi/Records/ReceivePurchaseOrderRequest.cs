namespace P2P.AgentApi.Records
{
    public sealed record ReceivePurchaseOrderRequest(
        string ReceivedBy,
        IReadOnlyList<ReceiveLineRequest> Lines
    );
}