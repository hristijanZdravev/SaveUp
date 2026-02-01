using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.DTOs;
using SaveUp.Models;
using SaveUp.Repository.Interfaces;

namespace SaveUp.Repository
{
    public class CurrencyRepository : ICommonRepository<Currency, CurrencyDTO>
    {
        private readonly Context _context;

        public CurrencyRepository(Context context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Currencies.AnyAsync(c => c.Id == id);
        }

        public async Task<Currency?> GetByIdAsync(int id)
        {
            return await _context.Currencies.FindAsync(id);
        }

        public async Task<List<CurrencyDTO>> GetAllAsync()
        {
            List<Currency> currencies = await _context.Currencies.ToListAsync();

            return currencies.Select(c => CurrencyDTO.toDTO(c)).ToList();
        }

        public async Task<bool> ExistAllAsync(List<int> ids)
        {
            var count = await _context.Currencies.CountAsync(c => ids.Contains(c.Id));
            return count == ids.Count;
        }
    }
}
