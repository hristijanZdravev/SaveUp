using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.DTOs;

namespace SaveUp.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly Context _context;

        public DashboardController(Context context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User?.Identity?.Name ?? throw new UnauthorizedAccessException();

        // =====================================
        // DASHBOARD STATS (range)
        // =====================================
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats([FromQuery] int days = 7)
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

            // 🔥 Volume = total reps
            var totalVolume = await _context.WorkoutSets
                .Where(s => workoutIds.Contains(s.WorkoutExercise.WorkoutId))
                .SumAsync(s => s.Reps);

            // COOL EXTRA → active workout days
            var activeDays = await workoutsQuery
                .Select(w => w.Date.Date)
                .Distinct()
                .CountAsync();

            return Ok(new
            {
                totalWorkouts,
                totalSets,
                totalVolume,
                activeDays
            });
        }

        // =====================================
        // RECENT WORKOUTS
        // =====================================
        [HttpGet("recent-workouts")]
        public async Task<IActionResult> GetRecent([FromQuery] int limit = 5)
        {
            var userId = GetUserId();

            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Take(limit)
                .Select(w => new
                {
                    w.Id,
                    w.Title,
                    w.Date
                })
                .ToListAsync();

            return Ok(workouts);
        }
    }

}
