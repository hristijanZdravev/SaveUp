using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.DTOs;
using SaveUp.Models.Transactions;
using SaveUp.Repository.Interfaces;

namespace SaveUp.Repository
{
    public class FeeCalculationHistoryRepository : IFeeCalculationHistoryRepository
    {
        private readonly Context _context;

        public FeeCalculationHistoryRepository(Context context)
        {
            _context = context;
        }

        public async Task AddAsync(FeeCalculationHistory history)
        {
            try
            {
                _context.FeeCalculationHistories.Add(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving fee calculation history", ex);
            }
        }

        public async Task AddRangeAsync(List<FeeCalculationHistory> histories)
        {
            try{
                _context.FeeCalculationHistories.AddRange(histories);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving fee calculation histories", ex);
            }
        }

        public async Task<List<FeeCalculationHistoryDTO>> GetAllAsync()
        {
            List<FeeCalculationHistory> feeCalculationHistory = await _context.FeeCalculationHistories
                        .OrderByDescending(h => h.Timestamp)
                        .Take(1000)
                        .Include(fc => fc.Transaction)
                        .Include(fc => fc.FeeRules)
                        .AsSplitQuery()
                        .ToListAsync();

            return feeCalculationHistory.Select(f => FeeCalculationHistoryDTO.toDTO(f)).ToList();
        }
    }
}
