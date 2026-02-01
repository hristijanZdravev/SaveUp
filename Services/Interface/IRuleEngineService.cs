using SaveUp.DTOs;
using SaveUp.Models;

namespace SaveUp.Services.Implementations
{
    public interface IRuleEngineService
    {
        TransactionResponseDTO Evaluate(TransactionRequestDTO tx, List<FeeRule> rules);
    }
}
