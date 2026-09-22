using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Entities
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; private set; }

        [Column("vendor_id")]
        public int VendorId { get; private set; }

        [Column("status")]
        public PurchaseOrderStatus Status { get; private set; } = PurchaseOrderStatus.DRAFT;

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        public Vendor Vendor { get; private set; } = null!;

        public ICollection<POLineItem> LineItems { get; private set; } = [];

        public ICollection<GoodsReceipt> GoodsReceipts { get; private set; } = [];

        public ICollection<Invoice> Invoices { get; private set; } = [];
    }
}