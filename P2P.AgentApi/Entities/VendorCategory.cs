using System.ComponentModel.DataAnnotations;

namespace P2P.AgentApi.Entities
{
    public class VendorCategory
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}