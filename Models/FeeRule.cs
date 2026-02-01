using System.ComponentModel.DataAnnotations;

namespace SaveUp.Models
{
    public class FeeRule
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }

        // A logical condition
        public required string ConditionExpression { get; set; }

        // A calculation formula
        public required string CalculationExpression { get; set; }

        public double? MaxFee { get; set; } // Optional max cap
        public double? DiscountPercent { get; set; } // Optional discount

        public List<FeeCalculationHistory> FeeCalculationHistories { get; set; } = new List<FeeCalculationHistory>();
    }

}
