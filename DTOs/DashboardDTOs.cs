namespace SaveUp.DTOs
{
    // =========================
    // DASHBOARD
    // =========================

    public class DashboardStatsDto
    {
        public int TotalWorkouts { get; set; }
        public int TotalSets { get; set; }
        public decimal TotalVolume { get; set; }
    }

    public class RecentWorkoutDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
