using PeakLift.Models;

namespace PeakLift.Repository
{
    public interface IWorkoutRepository
    {
        Task<int> CountByUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<Workout>> GetPagedByUserAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Workout?> GetByIdForUserAsync(Guid id, string userId, CancellationToken cancellationToken = default);
        Task<Workout?> GetWithGraphForUserAsync(Guid id, string userId, CancellationToken cancellationToken = default);
        Task AddAsync(Workout workout, CancellationToken cancellationToken = default);
        void Remove(Workout workout);
        Task<List<Workout>> FilterByDaysAsync(string userId, DateTime fromDate, CancellationToken cancellationToken = default);
        Task<List<Workout>> GetRecentByUserAsync(string userId, int limit, CancellationToken cancellationToken = default);
        IQueryable<Workout> QueryByUser(string userId);
    }
}
