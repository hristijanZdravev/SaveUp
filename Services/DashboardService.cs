using Microsoft.EntityFrameworkCore;
using PeakLift.DTOs;
using PeakLift.Repository;

namespace PeakLift.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IWorkoutRepository _workoutRepository;
        private readonly IWorkoutSetRepository _workoutSetRepository;

        public DashboardService(IWorkoutRepository workoutRepository, IWorkoutSetRepository workoutSetRepository)
        {
            _workoutRepository = workoutRepository;
            _workoutSetRepository = workoutSetRepository;
        }

        public async Task<DashboardStatsDto> GetStatsAsync(string userId, int days, CancellationToken cancellationToken = default)
        {
            var workoutsQuery = _workoutRepository.QueryByUser(userId);
            if (days > 0)
            {
                var fromDate = DateTime.UtcNow.AddDays(-days);
                workoutsQuery = workoutsQuery.Where(w => w.Date >= fromDate);
            }

            var workoutIds = await workoutsQuery
                .Select(w => w.Id)
                .ToListAsync(cancellationToken);

            var totalSets = await _workoutSetRepository.CountByWorkoutIdsAsync(workoutIds, cancellationToken);
            var totalVolume = await _workoutSetRepository.SumRepsByWorkoutIdsAsync(workoutIds, cancellationToken);
            var activeDays = await workoutsQuery
                .Select(w => w.Date.Date)
                .Distinct()
                .CountAsync(cancellationToken);

            return new DashboardStatsDto
            {
                TotalWorkouts = workoutIds.Count,
                TotalSets = totalSets,
                TotalVolume = totalVolume,
                ActiveDays = activeDays
            };
        }

        public async Task<List<RecentWorkoutDto>> GetRecentAsync(string userId, int limit, CancellationToken cancellationToken = default)
        {
            var workouts = await _workoutRepository.GetRecentByUserAsync(userId, limit, cancellationToken);
            return workouts.Select(w => new RecentWorkoutDto
            {
                Id = w.Id,
                Title = w.Title,
                Date = w.Date
            }).ToList();
        }
    }
}
