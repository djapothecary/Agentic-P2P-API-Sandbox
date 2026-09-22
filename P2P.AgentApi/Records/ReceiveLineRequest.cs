namespace P2P.AgentApi.Records
{
    public sealed record ReceiveLineRequest(
        int POLineItemId,
        int QuantityReceived
    );
}