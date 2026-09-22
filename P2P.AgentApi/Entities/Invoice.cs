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
        public int VendorId { get; init; }

        [Column("po_id")]
        public int PurchaseOrderId { get; init; }

        [Column("invoice_number")]
        public string InvoiceNumber { get; init; } = string.Empty;

        [Column("amount")]
        public decimal Amount { get; init; }

        [Column("status")]
        public InvoiceStatus InvoiceStatus { get; init; } = InvoiceStatus.PENDING;

        public Vendor Vendor { get; init; } = null!;

        public PurchaseOrder PurchaseOrder { get; init; } = null!;

        public ICollection<GLEntry> GLEntries { get; init; } = new List<GLEntry>();
    }
}