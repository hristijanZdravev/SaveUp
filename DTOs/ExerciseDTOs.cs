namespace PeakLift.DTOs
{
    public class ExerciseBodyGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class ExerciseListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public ExerciseBodyGroupDto BodyGroup { get; set; } = null!;
    }

    public class ExerciseByBodyPartDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public ExerciseBodyGroupDto BodyGroup { get; set; } = null!;
    }

    public class ExerciseSearchDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
