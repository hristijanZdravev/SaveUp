using System.ComponentModel.DataAnnotations;

namespace SaveUp.Models
{
    public class ClientSegment
    {
        [Key]
        public int Id { get; set; } 

        public required string Name { get; set; }
        List<Client> Clients { get; set; } = new List<Client>();
    }
}
