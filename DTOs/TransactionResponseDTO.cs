using System.ComponentModel.DataAnnotations;
using SaveUp.Models;

namespace SaveUp.DTOs
{
    public class TransactionResponseDTO
    {
        public double Fee { get; set; } = 0.0;

        public  List<int> FeeRuleIds { get; set; } = new List<int>();

        public  List<string> FeeRuleNames { get; set; } = new List<string>();

        public TransactionResponseDTO()
        {
        }
    }
}
