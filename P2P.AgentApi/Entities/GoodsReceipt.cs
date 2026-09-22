using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class GoodsReceipt
    {
        [Key]
        public int Id { get; private set; }

        [Column("po_id")]
        public int PurchaseOrderId { get; private set; }

        [Column("received_by")]
        public string ReceivedBy { get; private set; } = string.Empty;

        [Column("received_at")]
        public DateTime ReceivedAt { get; private set; }

        public PurchaseOrder PurchaseOrder { get; private set; } = null!;

        public ICollection<GoodsReceiptLine> Lines { get; private set; } = new List<GoodsReceiptLine>();
    }
}