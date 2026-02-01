using SaveUp.DTOs;
using SaveUp.Models;

namespace SaveUp.Services.Implementations
{
    public interface IFeeCalculationService
    {
        Task<TransactionResponseDTO> CalculateFeeAsync(TransactionRequestDTO request);
        Task<List<TransactionResponseDTO>> CalculateFeesBatchAsync(List<TransactionRequestDTO> requests);
        Task<List<FeeCalculationHistoryDTO>> GetCalculationHistoryAsync();
        Task<bool> ValidateTransactionRequestAsync(TransactionRequestDTO request);
        Task<bool> ValidateTransactionRequestsAsync(List<TransactionRequestDTO> requests);
    }
}
