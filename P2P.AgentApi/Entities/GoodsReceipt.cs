using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class GoodsReceipt
    {
        [Key]
        public int Id { get; set; }

        [Column("po_id")]
        public int PurchaseOrderId { get; set; }

        [Column("received_by")]
        public string ReceivedBy { get; set; } = string.Empty;

        [Column("received_at")]
        public DateTime ReceivedAt { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        public ICollection<GoodsReceiptLine> Lines { get; set; } = new List<GoodsReceiptLine>();
    }
}