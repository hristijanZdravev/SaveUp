using SaveUp.Models;

namespace SaveUp.Repository.Interfaces
{
    public interface IFeeRuleRepository
    {
        Task<List<FeeRule>> GetAllAsync();
        Task<FeeRule?> GetByIdAsync(int id);
        Task AddAsync(FeeRule feeRule);
        Task UpdateAsync(FeeRule feeRule);
        Task DeleteAsync(int id);

        Task<List<FeeRule>> GetByIdsAsync(List<int> ids);
    }
}
