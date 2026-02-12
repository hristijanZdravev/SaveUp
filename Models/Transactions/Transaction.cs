using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaveUp.Models.Transactions
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public required int TransactionTypeId { get; set; }

        [ForeignKey("TransactionTypeId")]
        public TransactionType? TransactionType { get; set; }

        public required double Amount { get; set; }

        public required int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency? Currency { get; set; }

        public bool IsDomestic { get; set; }

        public required int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; } = null!;

    }
}
