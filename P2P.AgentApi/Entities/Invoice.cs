using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Entities
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Column("vendor_id")]
        public int VendorId { get; set; }

        [Column("po_id")]
        public int PurchaseOrderId { get; set; }

        [Column("invoice_number")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("status")]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.PENDING;

        public Vendor Vendor { get; set; } = null!;

        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        public ICollection<GLEntry> GLEntries { get; set; } = [];

        public void MarkMatches()
        {
            if (Status != InvoiceStatus.PENDING)
            {
                throw new InvalidOperationException(
                    "Only pending invoices can be matched."
                );
            }

            Status = InvoiceStatus.MATCHED;
        }

        public void Approve()
        {
            if (Status != InvoiceStatus.MATCHED)
            {
                throw new InvalidOperationException(
                    "Only matched invoices can be approved."
                );
            }

            Status = InvoiceStatus.APPROVED;
        }
    }
}