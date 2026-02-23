using System.ComponentModel.DataAnnotations;

namespace PeakLift.Models
{
    public class Workout
    {
        public Guid Id { get; set; }

        public required string UserId { get; set; }
        public DateTime Date { get; set; }

        public string Title { get; set; } = null!; // "Push Day", "Leg Day"

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }

}
