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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Exercises
                .Include(e => e.BodyGroup)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var exercise = await _context.Exercises
                .Include(e => e.BodyGroup)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exercise == null) return NotFound();

            return Ok(exercise);
        }
    }

}
