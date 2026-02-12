using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaveUp.Models.Transactions
{
    public class FeeCalculationHistory
    {
        [Key]
        public int Id { get; set; }
        public required DateTime Timestamp { get; set; }

        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public required Transaction Transaction { get; set; }

        public double CalculatedFee { get; set; }
        public List<FeeRule> FeeRules { get; set; } = new List<FeeRule>();
        // Transaction that was processed
    }
}
