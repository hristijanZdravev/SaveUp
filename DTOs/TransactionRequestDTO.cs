using System.ComponentModel.DataAnnotations;
using SaveUp.Models.Transactions;

namespace SaveUp.DTOs
{
    public class TransactionRequestDTO
    {
        [Required(ErrorMessage = "Transaction type is required.")]
        public required TransactionTypeDTO Type { get; set; }

        public double Amount { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public required CurrencyDTO Currency { get; set; }

        public bool IsDomestic { get; set; }

        [Required(ErrorMessage = "Client information is required.")]
        public required ClientDTO Client { get; set; }

        public static Transaction ToTransaction(TransactionRequestDTO transactionRequestDTO)
        {
            return new Transaction
            {
                TransactionTypeId = transactionRequestDTO.Type.Id,
                Amount = transactionRequestDTO.Amount,
                CurrencyId = transactionRequestDTO.Currency.Id,
                IsDomestic = transactionRequestDTO.IsDomestic,
                ClientId = transactionRequestDTO.Client.Id
            };
        }
    }
}
