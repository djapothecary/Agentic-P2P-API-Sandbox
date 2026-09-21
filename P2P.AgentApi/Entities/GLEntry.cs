using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2P.AgentApi.Entities
{
    public class GLEntry
    {
        [Key]
        public int Id { get; init; }

        [Column("invoice_id")]
        public int InvoiceId { get; init; }

        [Column("account_code")]
        public string AccountCode { get; init; } = string.Empty;

        [Column("debit")]
        public double Debit { get; init; }

        [Column("credit")]
        public double Credit { get; init; }

        [Column("posted_at")]
        public DateTime PostedAt { get; init; }
    }
}