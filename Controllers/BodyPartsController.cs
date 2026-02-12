using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;

namespace SaveUp.Controllers
{
    [ApiController]
    [Route("api/body-parts")]
    [Authorize]
    public class BodyPartsController : ControllerBase
    {
        private readonly Context _context;

        public BodyPartsController(Context context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User?.Identity?.Name
            ?? throw new UnauthorizedAccessException();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.BodyGroups.ToListAsync());
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetStats(Guid id)
        {
            var userId = GetUserId();

            var totalSets = await _context.WorkoutSets
                .Where(s =>
                    s.WorkoutExercise.Workout.UserId == userId &&
                    s.WorkoutExercise.Exercise.BodyGroupId == id)
                .CountAsync();

            return Ok(new { totalSets });
        }
    }

}
