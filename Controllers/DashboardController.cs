using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;

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
            User?.Identity?.Name
            ?? throw new UnauthorizedAccessException();

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = GetUserId();

            var totalWorkouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .CountAsync();

            var totalSets = await _context.WorkoutSets
                .Where(s => s.WorkoutExercise.Workout.UserId == userId)
                .CountAsync();

            var totalVolume = await _context.WorkoutSets
                .Where(s => s.WorkoutExercise.Workout.UserId == userId)
                .SumAsync(s => s.Weight * s.Reps);

            return Ok(new
            {
                totalWorkouts,
                totalSets,
                totalVolume
            });
        }

        [HttpGet("recent-workouts")]
        public async Task<IActionResult> GetRecent([FromQuery] int limit = 5)
        {
            var userId = GetUserId();

            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Take(limit)
                .ToListAsync();

            return Ok(workouts);
        }
    }

}
