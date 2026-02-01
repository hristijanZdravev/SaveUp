using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.DTOs;
using SaveUp.Models;
using SaveUp.Repository.Interfaces;

namespace SaveUp.Repository
{
    public class TransactionTypeRepository : ICommonRepository<TransactionType, TransactionTypeDTO>
    {
        private readonly Context _context;

        public TransactionTypeRepository(Context context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TransactionTypes.AnyAsync(c => c.Id == id);
        }

        public async Task<TransactionType?> GetByIdAsync(int id)
        {
            return await _context.TransactionTypes.FindAsync(id);
        }

        public async Task<List<TransactionTypeDTO>> GetAllAsync()
        {
            List<TransactionType> transactionTypes = await _context.TransactionTypes.ToListAsync();

            return transactionTypes.Select(t => TransactionTypeDTO.toDTO(t)).ToList();
        }

        public async Task<bool> ExistAllAsync(List<int> ids)
        {
            var count = await _context.TransactionTypes.CountAsync(c => ids.Contains(c.Id));
            return count == ids.Count;
        }
    }
}
