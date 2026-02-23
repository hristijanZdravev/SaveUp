namespace PeakLift.Dtos
{
    public class AddExerciseDto
    {
        public Guid ExerciseId { get; set; }
    }

    public class AddExerciseResultDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
    }

    public class WorkoutExerciseDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public Guid ExerciseId { get; set; }
        public string ExerciseTitle { get; set; } = null!;
        public List<WorkoutSetDto> Sets { get; set; } = new();
    }
}
