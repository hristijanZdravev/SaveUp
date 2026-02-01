using SaveUp.Models;

namespace SaveUp.DTOs
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public int TransactionTypeId { get; set; }
        public double Amount { get; set; }

        public int CurrencyId { get; set; }

        public bool IsDomestic { get; set; }

        public int ClientId { get; set; }

        internal TransactionDTO toDTO(Transaction transaction)
        {
            return new TransactionDTO
            {
                Id = transaction.Id,
                TransactionTypeId = transaction.TransactionTypeId,
                Amount = transaction.Amount,
                CurrencyId = transaction.CurrencyId,
                IsDomestic = transaction.IsDomestic,
                ClientId = transaction.ClientId,

            };
        }
    }
}