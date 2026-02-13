using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
using SaveUp.Dtos;
using SaveUp.Models;

namespace SaveUp.Controllers
{
    [ApiController]
    [Route("api/workouts")]
    [Authorize]
    public class WorkoutsController : ControllerBase
    {
        private readonly Context _context;

        public WorkoutsController(Context context)
        {
            _context = context;
        }

        // IMPORTANT:
        // If you're using Keycloak, prefer: User.FindFirst("sub")?.Value
        // but keeping your current approach as requested:
        private string GetUserId() =>
            User?.Identity?.Name ?? throw new UnauthorizedAccessException();

        // =========================
        // GET ALL WORKOUTS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page,
            [FromQuery] int pageSize)
        {
            var userId = GetUserId();

            var query = _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date);

            var total = await query.CountAsync();

            var workouts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WorkoutListDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    Date = w.Date
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                total,
                data = workouts
            });
        }


        // =========================
        // GET WORKOUT DETAILS
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();

            var workout = await _context.Workouts
                .Where(w => w.Id == id && w.UserId == userId)
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync();

            if (workout == null)
                return NotFound();

            var result = new WorkoutDetailsDto
            {
                Id = workout.Id,
                Title = workout.Title,
                Date = workout.Date,
                WorkoutExercises = workout.WorkoutExercises
                    .OrderBy(we => we.Order)
                    .Select(we => new WorkoutExerciseDto
                    {
                        Id = we.Id,
                        ExerciseId = we.ExerciseId,
                        ExerciseTitle = we.Exercise.Title,
                        Sets = we.Sets
                            .OrderBy(s => s.SetNumber)
                            .Select(s => new WorkoutSetDto
                            {
                                Id = s.Id,
                                SetNumber = s.SetNumber,
                                Reps = s.Reps,
                                Weight = (decimal)s.Weight
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return Ok(result);
        }

        // =========================
        // CREATE WORKOUT
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkoutCreateDto dto)
        {
            var workout = new Workout
            {
                Id = Guid.NewGuid(),
                UserId = GetUserId(),
                Title = dto.Title,
                Date = dto.Date
            };

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            return Ok(new WorkoutListDto
            {
                Id = workout.Id,
                Title = workout.Title,
                Date = workout.Date
            });
        }

        // =========================
        // UPDATE WORKOUT
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] WorkoutUpdateDto dto)
        {
            var userId = GetUserId();

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound();

            workout.Title = dto.Title;
            workout.Date = dto.Date;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // =========================
        // DELETE WORKOUT
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound();

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // ADD EXERCISE TO WORKOUT
        // =========================
        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> AddExercise(Guid workoutId, [FromBody] AddExerciseDto dto)
        {
            var userId = GetUserId();

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

            if (workout == null)
                return NotFound();

            var workoutExercise = new WorkoutExercise
            {
                Id = Guid.NewGuid(),
                WorkoutId = workoutId,
                ExerciseId = dto.ExerciseId,
                Order = await _context.WorkoutExercises.CountAsync(x => x.WorkoutId == workoutId) + 1
            };

            _context.WorkoutExercises.Add(workoutExercise);
            await _context.SaveChangesAsync();

            var exerciseTitle = await _context.Exercises
                .Where(e => e.Id == dto.ExerciseId)
                .Select(e => e.Title)
                .FirstAsync();

            return Ok(new WorkoutExerciseDto
            {
                Id = workoutExercise.Id,
                ExerciseId = workoutExercise.ExerciseId,
                ExerciseTitle = exerciseTitle,
                Sets = []
            });
        }

        // =========================
        // DELETE EXERCISE FROM WORKOUT
        // =========================
        [HttpDelete("exercises/{workoutExerciseId}")]
        public async Task<IActionResult> DeleteExercise(Guid workoutExerciseId)
        {
            var userId = GetUserId();

            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.Id == workoutExerciseId);

            if (workoutExercise == null || workoutExercise.Workout.UserId != userId)
                return NotFound();

            _context.WorkoutExercises.Remove(workoutExercise);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // ADD SET
        // =========================
        [HttpPost("exercises/{workoutExerciseId}/sets")]
        public async Task<IActionResult> AddSet(Guid workoutExerciseId, [FromBody] AddSetDto dto)
        {
            var userId = GetUserId();

            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.Id == workoutExerciseId);

            if (workoutExercise == null || workoutExercise.Workout.UserId != userId)
                return NotFound();

            var set = new WorkoutSet
            {
                Id = Guid.NewGuid(),
                WorkoutExerciseId = workoutExerciseId,
                SetNumber = dto.SetNumber,
                Reps = dto.Reps,
                Weight = dto.Weight
            };

            _context.WorkoutSets.Add(set);
            await _context.SaveChangesAsync();

            return Ok(new WorkoutSetDto
            {
                Id = set.Id,
                SetNumber = set.SetNumber,
                Reps = set.Reps,
                Weight = (decimal)set.Weight
            });
        }

        // =========================
        // UPDATE SET
        // =========================
        [HttpPut("sets/{setId}")]
        public async Task<IActionResult> UpdateSet(Guid setId, [FromBody] UpdateSetDto dto)
        {
            var userId = GetUserId();

            var set = await _context.WorkoutSets
                .Include(s => s.WorkoutExercise)
                .ThenInclude(we => we.Workout)
                .FirstOrDefaultAsync(s => s.Id == setId);

            if (set == null || set.WorkoutExercise.Workout.UserId != userId)
                return NotFound();

            set.Reps = dto.Reps;
            set.Weight = dto.Weight;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // =========================
        // DELETE SET
        // =========================
        [HttpDelete("sets/{setId}")]
        public async Task<IActionResult> DeleteSet(Guid setId)
        {
            var userId = GetUserId();

            var set = await _context.WorkoutSets
                .Include(s => s.WorkoutExercise)
                .ThenInclude(we => we.Workout)
                .FirstOrDefaultAsync(s => s.Id == setId);

            if (set == null || set.WorkoutExercise.Workout.UserId != userId)
                return NotFound();

            _context.WorkoutSets.Remove(set);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterByDays([FromQuery] int days = 7)
        {
            var userId = GetUserId();

            if (days <= 0)
                return BadRequest("Days must be greater than 0");

            var startDate = DateTime.UtcNow.AddDays(-days);

            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId && w.Date >= startDate)
                .OrderByDescending(w => w.Date)
                .Select(w => new WorkoutListDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    Date = w.Date
                })
                .ToListAsync();

            return Ok(workouts);
        }


    }



}
