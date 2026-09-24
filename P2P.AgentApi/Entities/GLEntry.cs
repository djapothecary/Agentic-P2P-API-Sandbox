using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class GLEntry
    {
        [Key]
        public int Id { get; set; }

        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("account_code")]
        public string AccountCode { get; set; } = string.Empty;

        [Column("debit")]
        public decimal Debit { get; set; }

        [Column("credit")]
        public decimal Credit { get; set; }

        [Column("posted_at")]
        public DateTime PostedAt { get; set; }

        public Invoice Invoice { get; set; } = null!;

        public int AccountId { get; set; }
    }
}