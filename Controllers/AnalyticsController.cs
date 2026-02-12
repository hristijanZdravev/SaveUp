using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;

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
            User?.Identity?.Name
            ?? throw new UnauthorizedAccessException();

        [HttpGet("overview")]
        public async Task<IActionResult> Overview()
        {
            var userId = GetUserId();

            var totalWorkouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .CountAsync();

            var totalVolume = await _context.WorkoutSets
                .Where(s => s.WorkoutExercise.Workout.UserId == userId)
                .SumAsync(s => s.Weight * s.Reps);

            var topExercise = await _context.WorkoutExercises
                .Where(we => we.Workout.UserId == userId)
                .GroupBy(we => we.Exercise.Title)
                .Select(g => new { Exercise = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                totalWorkouts,
                totalVolume,
                topExercise
            });
        }

        [HttpGet("workout-frequency")]
        public async Task<IActionResult> WorkoutFrequency()
        {
            var userId = GetUserId();

            var data = await _context.Workouts
                .Where(w => w.UserId == userId)
                .GroupBy(w => w.Date.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("body-part-distribution")]
        public async Task<IActionResult> BodyPartDistribution()
        {
            var userId = GetUserId();

            var data = await _context.WorkoutExercises
                .Where(we => we.Workout.UserId == userId)
                .GroupBy(we => we.Exercise.BodyGroup.Name)
                .Select(g => new { BodyPart = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(data);
        }
    }

}
