using PeakLift.Models;

namespace PeakLift.Repository
{
    public interface IWorkoutExerciseRepository
    {
        Task<int> CountByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken = default);
        Task AddAsync(WorkoutExercise workoutExercise, CancellationToken cancellationToken = default);
        Task<List<WorkoutExercise>> GetByWorkoutForUserAsync(Guid workoutId, string userId, CancellationToken cancellationToken = default);
        Task<WorkoutExercise?> GetByIdWithWorkoutAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<WorkoutExercise>> GetByWorkoutIdOrderedAsync(Guid workoutId, CancellationToken cancellationToken = default);
        void Remove(WorkoutExercise workoutExercise);
        IQueryable<WorkoutExercise> Query();
    }
}
