using SaveUp.DTOs;
using SaveUp.Models.Transactions;

namespace SaveUp.Repository.Interfaces
{
    public interface IFeeCalculationHistoryRepository
    {
        Task AddAsync(FeeCalculationHistory history);
        Task AddRangeAsync(List<FeeCalculationHistory> histories);
        Task<List<FeeCalculationHistoryDTO>> GetAllAsync();
    }
}
