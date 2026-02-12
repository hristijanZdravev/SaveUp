using SaveUp.DTOs;
using SaveUp.Models.Transactions;

namespace SaveUp.Services.Implementations
{
    public interface IRuleEngineService
    {
        TransactionResponseDTO Evaluate(TransactionRequestDTO tx, List<FeeRule> rules);
    }
}
