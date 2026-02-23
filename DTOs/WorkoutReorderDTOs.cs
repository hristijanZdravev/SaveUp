namespace PeakLift.Dtos
{
    public class ReorderWorkoutExercisesDto
    {
        public List<ReorderItemDto> Items { get; set; } = new();
    }

    public class ReorderItemDto
    {
        public Guid WorkoutExerciseId { get; set; }
        public int Order { get; set; }
    }
}
