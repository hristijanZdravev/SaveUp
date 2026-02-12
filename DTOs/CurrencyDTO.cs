using System.ComponentModel.DataAnnotations;
using SaveUp.Models.Transactions;

namespace SaveUp.DTOs
{
    public class CurrencyDTO
    {
        [Required(ErrorMessage = "Id of Currency is required.")]
        public required int Id { get; set; }

        [Required(ErrorMessage = "Name of Currency is required.")]
        public required string Name { get; set; }

        public static CurrencyDTO toDTO(Currency c)
        {
            return new CurrencyDTO
            {
                Id = c.Id,
                Name = c.Name
            };
        }
    }
}