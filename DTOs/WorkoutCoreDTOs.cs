namespace PeakLift.Dtos
{
    public class WorkoutCreateDto
    {
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
    }

    public class WorkoutListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
    }

    public class WorkoutPagedResultDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<WorkoutListDto> Data { get; set; } = new();
    }

    public class WorkoutDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
        public List<WorkoutExerciseDto> Exercises { get; set; } = new();
    }

    public class WorkoutUpdateDto
    {
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
