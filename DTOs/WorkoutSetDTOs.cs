namespace PeakLift.Dtos
{
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
}
