using SaveUp.Models;

namespace SaveUp.DTOs
{
    public class FeeRuleDTO
    {
       public int Id { get; set; }
       public string? Name { get; set; }

        internal FeeRuleDTO toDTO(FeeRule fr)
        {
            return new FeeRuleDTO
            {
                Id = fr.Id,
                Name = fr.Name
            };
        }
    }
}