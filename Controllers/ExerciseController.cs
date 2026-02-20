using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.Services;
using System;

namespace SaveUp.Controllers
{
    [ApiController]
    [Route("api/exercises")]
    [Authorize]
    public class ExercisesController : ControllerBase
    {
        private readonly Context _context;
        private readonly CloudinaryService _cloudinary;

        public ExercisesController(Context context, CloudinaryService cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // =========================
        // GET: /api/exercises
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _context.Exercises
                .AsNoTracking()
                .Include(e => e.BodyGroup)
                .OrderBy(e => e.Title)
                .ToListAsync(); // 🔥 fetch first

            var result = exercises.Select(e => new
            {
                e.Id,
                e.Title,
                e.Description,
                ImageUrl = string.IsNullOrEmpty(e.ImagePublicId)
                    ? null
                    : _cloudinary.GetExerciseImage(e.ImagePublicId),
                BodyGroup = new
                {
                    e.BodyGroup.Id,
                    e.BodyGroup.Name
                }
            });

            return Ok(result);
        }

        // =========================
        // GET: /api/exercises/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var exercise = await _context.Exercises
                .AsNoTracking()
                .Include(e => e.BodyGroup)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exercise == null)
                return NotFound();

            var result = new
            {
                exercise.Id,
                exercise.Title,
                exercise.Description,
                ImageUrl = string.IsNullOrEmpty(exercise.ImagePublicId)
                    ? null
                    : _cloudinary.GetExerciseImage(exercise.ImagePublicId),
                BodyGroup = new
                {
                    exercise.BodyGroup.Id,
                    exercise.BodyGroup.Name
                }
            };

            return Ok(result);
        }

        // =========================
        // GET: /api/exercises/by-body-part/{bodyPart}
        // =========================
        [HttpGet("by-body-part/{bodyPart}")]
        public async Task<IActionResult> GetByBodyPart(string bodyPart)
        {
            bodyPart = bodyPart.ToLower();

            var exercises = await _context.Exercises
                .AsNoTracking()
                .Include(e => e.BodyGroup)
                .Where(e => e.BodyGroup.Name.ToLower() == bodyPart)
                .OrderBy(e => e.Title)
                .ToListAsync();

            var result = exercises.Select(e => new
            {
                e.Id,
                e.Title,
                ImageUrl = string.IsNullOrEmpty(e.ImagePublicId)
                    ? null
                    : _cloudinary.GetExerciseImage(e.ImagePublicId),
                BodyGroup = new
                {
                    e.BodyGroup.Id,
                    e.BodyGroup.Name
                }
            });

            return Ok(result);
        }

        // =========================
        // GET: /api/exercises/search?query=bench
        // =========================
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            query = (query ?? string.Empty).Trim();

            if (query.Length < 2)
                return Ok(Array.Empty<object>());

            var exercises = await _context.Exercises
                .AsNoTracking()
                .Where(e => EF.Functions.Like(e.Title, $"%{query}%"))
                .OrderBy(e => e.Title)
                .Take(20)
                .ToListAsync();

            var result = exercises.Select(e => new
            {
                e.Id,
                e.Title,
                ImageUrl = string.IsNullOrEmpty(e.ImagePublicId)
                    ? null
                    : _cloudinary.GetExerciseImage(e.ImagePublicId)
            });

            return Ok(result);
        }
    }
}
