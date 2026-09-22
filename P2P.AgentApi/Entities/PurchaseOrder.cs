using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Entities
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; init; }

        [Column("vendor_id")]
        public string VendorId { get; init; } = string.Empty;

        [Column("status")]
        public PurchaseOrderStatus Status { get; init; } = PurchaseOrderStatus.DRAFT;

        //  Becareful here!! this could be a string of multiple items comma seperated
        //  Be sure to split this into a list as needed
        [Column("line_items")]
        public string LineItems { get; init; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        public Vendor Vendor { get; init; } = null!;

        public ICollection<POLineItem> LineItemEntities { get; init; } = new List<POLineItem>();

        public ICollection<GoodsReceipt> GoodsReceipts { get; init; } = new List<GoodsReceipt>();

        public ICollection<Invoice> Invoices { get; init; } = new List<Invoice>();
    }
}