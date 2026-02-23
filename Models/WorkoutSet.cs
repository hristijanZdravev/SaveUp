namespace PeakLift.Models
{
    public class WorkoutSet
    {
        public Guid Id { get; set; }

        public Guid WorkoutExerciseId { get; set; }
        public WorkoutExercise WorkoutExercise { get; set; } = null!;

        public int SetNumber { get; set; }

        public int Reps { get; set; }
        public decimal? Weight { get; set; } // Optional
        public int? DurationSeconds { get; set; } // Optional (for cardio)
    }

}
