using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Entities
{
    public class Invoice
    {
        [Key]
        public int Id { get; init; }

        [Column("vendor_id")]
        public int VenodrId { get; init; }

        [Column("po_id")]
        public int PurchaseOrderId { get; init; }

        [Column("invoice_number")]
        public string InvoiceNumber { get; init; } = string.Empty;

        [Column("amount")]
        public double Amount { get; init; }

        [Column("status")]
        public InvoiceStatus InvoiceStatus { get; init; } = InvoiceStatus.PENDING;
    }
}