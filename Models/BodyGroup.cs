namespace SaveUp.Models
{
    public class BodyGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!; // Chest, Back, Legs
        public string? SubGroup { get; set; }     // Upper Chest, Lats, Hamstrings (optional)

        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }

}
