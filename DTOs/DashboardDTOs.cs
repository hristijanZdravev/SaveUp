namespace PeakLift.DTOs
{
    // =========================
    // DASHBOARD
    // =========================

    public class DashboardStatsDto
    {
        public int TotalWorkouts { get; set; }
        public int TotalSets { get; set; }
        public int TotalVolume { get; set; }
        public int ActiveDays { get; set; }
    }

    public class RecentWorkoutDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
