using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Entities
{
    public class Vendor
    {
        [Key]
        public int Id { get; init; }

        [Column("name")]
        public string Name { get; init; } = string.Empty;

        [Column("payment_terms")]
        public PaymentTerms PaymentTerms { get; init; } = PaymentTerms.NONE;

        [Column("is_active")]
        public bool IsActive { get; init; }

        public ICollection<PurchaseOrder> Purchaseorders { get; init; } = new List<PurchaseOrder>();

        public ICollection<Invoice> Invoices { get; init; } = new List<Invoice>();
    }
}