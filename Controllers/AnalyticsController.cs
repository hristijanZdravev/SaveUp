using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.DTOs;

namespace SaveUp.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly Context _context;

        public AnalyticsController(Context context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User?.Identity?.Name ?? throw new UnauthorizedAccessException();

        // =====================================
        // ANALYTICS SUMMARY
        // =====================================
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] int days = 7)
        {
            var userId = GetUserId();

            var workoutsQuery = _context.Workouts
                .Where(w => w.UserId == userId);

            if (days > 0)
            {
                var fromDate = DateTime.UtcNow.AddDays(-days);
                workoutsQuery = workoutsQuery.Where(w => w.Date >= fromDate);
            }

            var workoutIds = await workoutsQuery
                .Select(w => w.Id)
                .ToListAsync();

            var totalWorkouts = workoutIds.Count;

            var totalSets = await _context.WorkoutSets
                .Where(s => workoutIds.Contains(s.WorkoutExercise.WorkoutId))
                .CountAsync();

            // volume = total reps
            var totalVolume = await _context.WorkoutSets
                .Where(s => workoutIds.Contains(s.WorkoutExercise.WorkoutId))
                .SumAsync(s => s.Reps);

            var avgRepsPerSet = await _context.WorkoutSets
                .Where(s => workoutIds.Contains(s.WorkoutExercise.WorkoutId))
                .AverageAsync(s => (double?)s.Reps) ?? 0;

            var avgSetsPerWorkout =
                totalWorkouts == 0 ? 0 : (double)totalSets / totalWorkouts;

            // ⭐ NEW COOL METRIC → Consistency Score
            var activeDays = await workoutsQuery
                .Select(w => w.Date.Date)
                .Distinct()
                .CountAsync();

            int totalDaysInRange = days == 0
                ? Math.Max(activeDays, 1)
                : days;

            var consistencyScore =
                (double)activeDays / totalDaysInRange * 100;

            return Ok(new
            {
                totalWorkouts,
                totalSets,
                totalVolume,
                avgRepsPerSet,
                avgSetsPerWorkout,
                activeDays,
                consistencyScore
            });
        }

        // =====================================
        // SETS PER DAY
        // =====================================
        [HttpGet("sets-per-day")]
        public async Task<IActionResult> SetsPerDay([FromQuery] int days = 7)
        {
            var userId = GetUserId();

            var data = await _context.WorkoutSets
                .Where(s =>
                    s.WorkoutExercise.Workout.UserId == userId &&
                    (days == 0 ||
                     s.WorkoutExercise.Workout.Date >= DateTime.UtcNow.AddDays(-days)))
                .GroupBy(s => s.WorkoutExercise.Workout.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Sets = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return Ok(data);
        }

        // =====================================
        // BODY PART DISTRIBUTION
        // =====================================
        [HttpGet("body-part-distribution")]
        public async Task<IActionResult> BodyPartDistribution([FromQuery] int days = 7)
        {
            var userId = GetUserId();

            var data = await _context.WorkoutExercises
                .Where(we =>
                    we.Workout.UserId == userId &&
                    (days == 0 ||
                     we.Workout.Date >= DateTime.UtcNow.AddDays(-days)))
                .GroupBy(we => we.Exercise.BodyGroup.Name)
                .Select(g => new
                {
                    BodyPart = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
