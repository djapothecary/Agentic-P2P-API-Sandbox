namespace P2P.AgentApi.Entities
{
    public class CreateInvoiceRequest
    {
        public int PurchaseOrderId { get; set; }

        public decimal Amount { get; set; }
    }
}