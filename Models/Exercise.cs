namespace SaveUp.Models
{
    public class Exercise
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? ImagePublicId { get; set; } = null!;

        public Guid BodyGroupId { get; set; }
        public BodyGroup BodyGroup { get; set; } = null!;

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }

}
