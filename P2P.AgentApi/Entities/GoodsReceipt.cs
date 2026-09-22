using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class GoodsReceipt
    {
        [Key]
        public int Id { get; init; }

        [Column("po_id")]
        public int PurchaseOrderId { get; init; }

        [Column("sku")]
        public string Sku { get; init; } = string.Empty;

        [Column("description")]
        public string Description { get; init; } = string.Empty;

        [Column("qty_ordered")]
        public int QuantityOrdered { get; init; }

        [Column("qtyreceived")]
        public int QuantityReceived { get; init; }

        [Column("unit_cost")]
        public decimal UnitCost { get; init; }

        public PurchaseOrder PurchaseOrder { get; init; } = null!;
    }
}