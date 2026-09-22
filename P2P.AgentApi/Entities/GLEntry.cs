using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class GLEntry
    {
        [Key]
        public int Id { get; private set; }

        [Column("invoice_id")]
        public int InvoiceId { get; private set; }

        [Column("account_code")]
        public string AccountCode { get; private set; } = string.Empty;

        [Column("debit")]
        public decimal Debit { get; private set; }

        [Column("credit")]
        public decimal Credit { get; private set; }

        [Column("posted_at")]
        public DateTime PostedAt { get; private set; }

        public Invoice Invoice { get; private set; } = null!;
    }
}