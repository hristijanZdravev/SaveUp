using Microsoft.EntityFrameworkCore;
using PeakLift.DTOs;
using PeakLift.Repository;

namespace PeakLift.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IWorkoutRepository _workoutRepository;
        private readonly IWorkoutSetRepository _workoutSetRepository;
        private readonly IWorkoutExerciseRepository _workoutExerciseRepository;

        public AnalyticsService(
            IWorkoutRepository workoutRepository,
            IWorkoutSetRepository workoutSetRepository,
            IWorkoutExerciseRepository workoutExerciseRepository)
        {
            _workoutRepository = workoutRepository;
            _workoutSetRepository = workoutSetRepository;
            _workoutExerciseRepository = workoutExerciseRepository;
        }

        public async Task<AnalyticsSummaryDto> GetSummaryAsync(string userId, int days, CancellationToken cancellationToken = default)
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

            var totalWorkouts = workoutIds.Count;
            var totalSets = await _workoutSetRepository.CountByWorkoutIdsAsync(workoutIds, cancellationToken);
            var totalVolume = await _workoutSetRepository.SumRepsByWorkoutIdsAsync(workoutIds, cancellationToken);
            var avgRepsPerSet = await _workoutSetRepository.AverageRepsByWorkoutIdsAsync(workoutIds, cancellationToken);
            var avgSetsPerWorkout = totalWorkouts == 0 ? 0 : (double)totalSets / totalWorkouts;
            var activeDays = await workoutsQuery
                .Select(w => w.Date.Date)
                .Distinct()
                .CountAsync(cancellationToken);

            int totalDaysInRange = days == 0
                ? Math.Max(activeDays, 1)
                : days;

            var consistencyScore = (double)activeDays / totalDaysInRange * 100;

            return new AnalyticsSummaryDto
            {
                TotalWorkouts = totalWorkouts,
                TotalSets = totalSets,
                TotalVolume = totalVolume,
                AvgRepsPerSet = avgRepsPerSet,
                AvgSetsPerWorkout = avgSetsPerWorkout,
                ActiveDays = activeDays,
                ConsistencyScore = consistencyScore
            };
        }

        public Task<List<SetsPerDayDto>> GetSetsPerDayAsync(string userId, int days, CancellationToken cancellationToken = default)
        {
            return _workoutSetRepository.Query()
                .Where(s =>
                    s.WorkoutExercise.Workout.UserId == userId &&
                    (days == 0 || s.WorkoutExercise.Workout.Date >= DateTime.UtcNow.AddDays(-days)))
                .GroupBy(s => s.WorkoutExercise.Workout.Date.Date)
                .Select(g => new SetsPerDayDto
                {
                    Date = g.Key,
                    Sets = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToListAsync(cancellationToken);
        }

        public Task<List<BodyPartDistributionDto>> GetBodyPartDistributionAsync(string userId, int days, CancellationToken cancellationToken = default)
        {
            return _workoutExerciseRepository.Query()
                .Where(we =>
                    we.Workout.UserId == userId &&
                    (days == 0 || we.Workout.Date >= DateTime.UtcNow.AddDays(-days)))
                .GroupBy(we => we.Exercise.BodyGroup.Name)
                .Select(g => new BodyPartDistributionDto
                {
                    BodyPart = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
