namespace SaveUp.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SaveUp.Data;
    using System;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class ExerciseController : ControllerBase
    {
        private readonly Context _context;

        public ExerciseController(Context context)
        {
            _context = context;
        }

        // GET: api/exercise
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = User;

            var exercises = await _context.Exercises
                .ToListAsync();

            return Ok(exercises);
        }

        // GET: api/exercise/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var exercise = await _context.Exercises
                .Include(e => e.BodyGroup)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exercise == null)
                return NotFound();

            return Ok(exercise);
        }

        // GET: api/exercise/bodygroup/{bodyGroupId}
        [HttpGet("bodygroup/{bodyGroupId}")]
        public async Task<IActionResult> GetByBodyGroup(Guid bodyGroupId)
        {
            var exercises = await _context.Exercises
                .Where(e => e.BodyGroupId == bodyGroupId)
                .ToListAsync();

            return Ok(exercises);
        }
    }

}
