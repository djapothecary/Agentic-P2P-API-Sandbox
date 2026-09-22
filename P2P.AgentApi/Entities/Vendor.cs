using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Entities
{
    public class Vendor
    {
        [Key]
        public int Id { get; private set; }

        [Column("name")]
        public string Name { get; private set; } = string.Empty;

        [Column("payment_terms")]
        public PaymentTerms PaymentTerms { get; private set; } = PaymentTerms.NONE;

        [Column("is_active")]
        public bool IsActive { get; private set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; private set; } = [];

        public ICollection<Invoice> Invoices { get; private set; } = [];

        //  Constructor/Factory
        public Vendor(
            string name,
            PaymentTerms paymentTerms,
            bool isActive = true
        )
        {
            Name = name;
            PaymentTerms = paymentTerms;
            IsActive = isActive;
        }

        private Vendor()
        {
            //  Required by EF Core
        }
    }
}