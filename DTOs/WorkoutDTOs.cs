namespace SaveUp.Dtos
{
    // =========================
    // WORKOUT
    // =========================
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

    // =========================
    // WORKOUT EXERCISE
    // =========================
    public class AddExerciseDto
    {
        public Guid ExerciseId { get; set; }
    }

    public class WorkoutExerciseDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }

        public Guid ExerciseId { get; set; }
        public string ExerciseTitle { get; set; } = null!;

        public List<WorkoutSetDto> Sets { get; set; } = new();
    }

    // =========================
    // SETS
    // =========================
    public class AddSetDto
    {
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public decimal Weight { get; set; }
    }

    public class WorkoutSetDto
    {
        public Guid Id { get; set; }
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public decimal Weight { get; set; }
    }

    public class UpdateSetDto
    {
        public int Reps { get; set; }
        public decimal Weight { get; set; }
    }

    // =========================
    // REORDER
    // =========================
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