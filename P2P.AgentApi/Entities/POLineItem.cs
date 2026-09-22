using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class POLineItem
    {
        [Key]
        public int Id { get; private set; }

        [Column("po_id")]
        public int PurchaseOrderId { get; private set; }

        [Column("sku")]
        public string Sku { get; private set; } = string.Empty;

        [Column("description")]
        public string Description { get; private set; } = string.Empty;

        [Column("qty_ordered")]
        public int QuantityOrdered { get; private set; }

        [Column("unit_cost")]
        public decimal UnitCost { get; private set; }

        public PurchaseOrder PurchaseOrder { get; private set; } = null!;
    }
}