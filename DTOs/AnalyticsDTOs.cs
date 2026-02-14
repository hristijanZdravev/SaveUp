namespace SaveUp.DTOs
{

    // =========================
    // ANALYTICS
    // =========================

    public class AnalyticsOverviewDto
    {
        public int TotalWorkouts { get; set; }
        public decimal TotalVolume { get; set; }
        public TopExerciseDto? TopExercise { get; set; }
    }

    public class TopExerciseDto
    {
        public string Exercise { get; set; } = null!;
        public int Count { get; set; }
    }

    public class WorkoutFrequencyDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class BodyPartDistributionDto
    {
        public string BodyPart { get; set; } = null!;
        public int Count { get; set; }
    }
}
