using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class GoodsReceiptLine
    {
        [Key]
        public int Id { get; private set; }

        [Column("goods_receipt_id")]
        public int GoodsReceiptId { get; private set; }

        [Column("po_line_item_id")]
        public int POLineItemId { get; private set; }

        [Column("quantity_received")]
        public int QuantityReceived { get; private set; }

        public GoodsReceipt GoodsReceipt { get; private set; } = null!;

        public POLineItem POLineItem { get; private set; } = null!;
    }
}