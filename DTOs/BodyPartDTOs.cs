namespace SaveUp.DTOs
{
    // =========================
    // BODY PARTS
    // =========================

    public class BodyPartDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class BodyPartStatsDto
    {
        public int TotalSets { get; set; }
    }
}
