using System.ComponentModel.DataAnnotations;

namespace SaveUp.Models
{
    public class TransactionType
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
