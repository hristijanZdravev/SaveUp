using SaveUp.Models;

namespace SaveUp.Repository.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);

    }
}
