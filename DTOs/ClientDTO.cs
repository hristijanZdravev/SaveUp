using System.ComponentModel.DataAnnotations;
using SaveUp.Models.Enum;
using SaveUp.Models.Transactions;

namespace SaveUp.DTOs
{
    public class ClientDTO
    {
        [Required(ErrorMessage = "Id of Client is required.")]
        public required int Id { get; set; }

        public int CreditScore { get; set; }

        [Required(ErrorMessage = "ClientSegment is required.")]
        public required ClientSegmentDTO Segment { get; set; }
        public RiskLevel RiskLevel { get; set; }

        public static ClientDTO toDTO(Client c)
        {
            return new ClientDTO
            {
                Id = c.Id,
                CreditScore = c.CreditScore,
                Segment = ClientSegmentDTO.toDTO(c.ClientSegment!),
                RiskLevel = c.RiskLevel
            };
        }
    }
}
