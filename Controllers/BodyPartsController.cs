using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.DTOs;

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
            var bodyParts = await _context.BodyGroups
                .Select(b => new BodyPartDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();

            return Ok(bodyParts);
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetStats(Guid id)
        {
            var userId = GetUserId();

            var dto = new BodyPartStatsDto
            {
                TotalSets = await _context.WorkoutSets
                    .Where(s =>
                        s.WorkoutExercise.Workout.UserId == userId &&
                        s.WorkoutExercise.Exercise.BodyGroupId == id)
                    .CountAsync()
            };

            return Ok(dto);
        }
    }
}
