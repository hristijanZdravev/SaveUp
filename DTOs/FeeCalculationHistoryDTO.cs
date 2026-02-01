using SaveUp.Models;

namespace SaveUp.DTOs
{
    public class FeeCalculationHistoryDTO
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double CalculatedFee { get; set; }
        public required List<FeeRuleDTO> FeeRules { get; set; }
        
        public required TransactionDTO Transaction { get; set; }

        public static FeeCalculationHistoryDTO toDTO(FeeCalculationHistory feeCalculationHistory) {
            return new FeeCalculationHistoryDTO {
                Id = feeCalculationHistory.Id,
                Timestamp = feeCalculationHistory.Timestamp,
                CalculatedFee = feeCalculationHistory.CalculatedFee,
                FeeRules = feeCalculationHistory.FeeRules.Select(fr => new FeeRuleDTO().toDTO(fr)).ToList(),
                Transaction = new TransactionDTO().toDTO(feeCalculationHistory.Transaction)
            };
        }
    }
}
