using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Entities
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }

        [Column("vendor_id")]
        public int VendorId { get; set; }

        [Column("status")]
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.DRAFT;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public Vendor Vendor { get; set; } = null!;

        public ICollection<POLineItem> POLineItems { get; set; } = [];

        public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = [];

        public ICollection<Invoice> Invoices { get; set; } = [];

        //  Constructor/Factory
        public PurchaseOrder(
            int vendorId,
            DateTime? createdAt = null
        )
        {
            VendorId = vendorId;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            Status = PurchaseOrderStatus.DRAFT;
        }

        public PurchaseOrder()
        {
            //  Required by EF Core
        }
    }
}