using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SaveUp.Models.Enum;

namespace SaveUp.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public int CreditScore { get; set; }

        public required int ClientSegmentId { get; set; }

        [ForeignKey("ClientSegmentId")]
        public ClientSegment? ClientSegment { get; set; }
        public RiskLevel RiskLevel { get; set; }

        List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
