using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class POLineItem
    {
        [Key]
        public int Id { get; set; }

        [Column("po_id")]
        public int PurchaseOrderId { get; set; }

        [Column("sku")]
        public string Sku { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("qty_ordered")]
        public int QuantityOrdered { get; set; }

        [Column("qty_received")]
        public int QuantityReceived { get; set; }

        [Column("unit_cost")]
        public decimal UnitCost { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        //  Constructor/Factory
        public POLineItem(
            string sku,
            string description,
            int quantityOrdered,
            decimal unitCost
        )
        {
            Sku = sku;
            Description = description;
            QuantityOrdered = quantityOrdered;
            QuantityReceived = 0;
            UnitCost = unitCost;
        }

        public POLineItem()
        {
            //  Required by EF Core
        }
    }
}