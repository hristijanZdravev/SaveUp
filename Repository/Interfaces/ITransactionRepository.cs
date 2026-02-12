using SaveUp.Models.Transactions;

namespace SaveUp.Repository.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);

    }
}
