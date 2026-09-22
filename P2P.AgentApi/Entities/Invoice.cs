using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Entities
{
    public class Invoice
    {
        [Key]
        public int Id { get; private set; }

        [Column("vendor_id")]
        public int VendorId { get; private set; }

        [Column("po_id")]
        public int PurchaseOrderId { get; private set; }

        [Column("invoice_number")]
        public string InvoiceNumber { get; private set; } = string.Empty;

        [Column("amount")]
        public decimal Amount { get; private set; }

        [Column("status")]
        public InvoiceStatus Status { get; private set; } = InvoiceStatus.PENDING;

        public Vendor Vendor { get; private set; } = null!;

        public PurchaseOrder PurchaseOrder { get; private set; } = null!;

        public ICollection<GLEntry> GLEntries { get; private set; } = [];
    }
}