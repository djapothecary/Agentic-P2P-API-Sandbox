using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class GoodsReceiptLine
    {
        [Key]
        public int Id { get; set; }

        [Column("goods_receipt_id")]
        public int GoodsReceiptId { get; set; }

        [Column("po_line_item_id")]
        public int POLineItemId { get; set; }

        [Column("quantity_received")]
        public int QuantityReceived { get; set; }

        public GoodsReceipt GoodsReceipt { get; set; } = null!;

        public POLineItem POLineItem { get; set; } = null!;
    }
}