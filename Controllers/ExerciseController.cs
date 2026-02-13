namespace SaveUp.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SaveUp.Data;
    using System;

    [ApiController]
    [Route("api/exercises")]
    [Authorize]
    public class ExercisesController : ControllerBase
    {
        private readonly Context _context;

        public ExercisesController(Context context)
        {
            _context = context;
        }

        // GET: /api/exercises
        // Used for dropdown lists / exercise browser
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _context.Exercises
                .Include(e => e.BodyGroup)
                .OrderBy(e => e.Title)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    BodyGroup = new
                    {
                        e.BodyGroup.Id,
                        e.BodyGroup.Name
                    }
                })
                .ToListAsync();

            return Ok(exercises);
        }

        // GET: /api/exercises/{id}
        // Used for exercise details page/modal
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var exercise = await _context.Exercises
                .Include(e => e.BodyGroup)
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    BodyGroup = new
                    {
                        e.BodyGroup.Id,
                        e.BodyGroup.Name
                    }
                })
                .FirstOrDefaultAsync();

            if (exercise == null) return NotFound();
            return Ok(exercise);
        }

        // GET: /api/exercises/by-body-part/{bodyPart}
        // Used for UI filters like "Chest", "Back", etc.
        [HttpGet("by-body-part/{bodyPart}")]
        public async Task<IActionResult> GetByBodyPart(string bodyPart)
        {
            var exercises = await _context.Exercises
                .Include(e => e.BodyGroup)
                .Where(e => e.BodyGroup.Name == bodyPart)
                .OrderBy(e => e.Title)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    BodyGroup = new
                    {
                        e.BodyGroup.Id,
                        e.BodyGroup.Name
                    }
                })
                .ToListAsync();

            return Ok(exercises);
        }

        // GET: /api/exercises/search?query=bench
        // Used for real-time search/autocomplete in UI
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            query = (query ?? string.Empty).Trim();

            if (query.Length == 0)
                return Ok(Array.Empty<object>());

            var exercises = await _context.Exercises
                .Where(e => EF.Functions.Like(e.Title, $"%{query}%"))
                .OrderBy(e => e.Title)
                .Take(20)
                .Select(e => new
                {
                    e.Id,
                    e.Title
                })
                .ToListAsync();

            return Ok(exercises);
        }
    }

}
