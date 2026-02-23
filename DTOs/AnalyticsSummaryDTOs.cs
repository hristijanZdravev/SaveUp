namespace PeakLift.DTOs
{
    public class BodyPartDistributionDto
    {
        public string BodyPart { get; set; } = null!;
        public int Count { get; set; }
    }

    public class AnalyticsSummaryDto
    {
        public int TotalWorkouts { get; set; }
        public int TotalSets { get; set; }
        public int TotalVolume { get; set; }
        public double AvgRepsPerSet { get; set; }
        public double AvgSetsPerWorkout { get; set; }
        public int ActiveDays { get; set; }
        public double ConsistencyScore { get; set; }
    }

    public class SetsPerDayDto
    {
        public DateTime Date { get; set; }
        public int Sets { get; set; }
    }
}
