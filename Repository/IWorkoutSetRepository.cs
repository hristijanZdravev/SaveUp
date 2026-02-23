using PeakLift.Models;

namespace PeakLift.Repository
{
    public interface IWorkoutSetRepository
    {
        Task<int> CountByUserAndBodyGroupAsync(string userId, Guid bodyGroupId, CancellationToken cancellationToken = default);
        Task<int> CountByWorkoutIdsAsync(List<Guid> workoutIds, CancellationToken cancellationToken = default);
        Task<int> SumRepsByWorkoutIdsAsync(List<Guid> workoutIds, CancellationToken cancellationToken = default);
        Task<double> AverageRepsByWorkoutIdsAsync(List<Guid> workoutIds, CancellationToken cancellationToken = default);
        Task<WorkoutSet?> GetByIdWithOwnershipAsync(Guid setId, string userId, CancellationToken cancellationToken = default);
        Task<WorkoutSet?> FindByIdAsync(Guid setId, CancellationToken cancellationToken = default);
        Task AddAsync(WorkoutSet workoutSet, CancellationToken cancellationToken = default);
        void Remove(WorkoutSet workoutSet);
        IQueryable<WorkoutSet> Query();
    }
}
