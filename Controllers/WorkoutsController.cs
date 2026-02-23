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

        private string GetUserId() =>
            User?.Identity?.Name ?? throw new UnauthorizedAccessException();

        // =========================
        // GET ALL (PAGINATED)
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var userId = GetUserId();

            var query = _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date);

            var total = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WorkoutListDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    Date = w.Date
                })
                .ToListAsync();

            return Ok(new { page, pageSize, total, data });
        }

        // =========================
        // GET DETAILS (SAFE DTO)
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();

            var workout = await _context.Workouts
                .Where(w => w.Id == id && w.UserId == userId)
                .Select(w => new WorkoutDetailsDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    Date = w.Date,
                    Exercises = w.WorkoutExercises
                        .OrderBy(we => we.Order)
                        .Select(we => new WorkoutExerciseDto
                        {
                            Id = we.Id,
                            Order = we.Order,
                            ExerciseId = we.ExerciseId,
                            ExerciseTitle = we.Exercise.Title,
                            Sets = we.Sets
                                .OrderBy(s => s.SetNumber)
                                .Select(s => new WorkoutSetDto
                                {
                                    Id = s.Id,
                                    SetNumber = s.SetNumber,
                                    Reps = s.Reps,
                                    Weight = (decimal)s.Weight!
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (workout == null)
                return NotFound();

            return Ok(workout);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(WorkoutCreateDto dto)
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
        // ADD EXERCISE
        // =========================
        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> AddExercise(Guid workoutId, AddExerciseDto dto)
        {
            var userId = GetUserId();

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

            if (workout == null) return NotFound();

            var order = await _context.WorkoutExercises
                .Where(x => x.WorkoutId == workoutId)
                .CountAsync() + 1;

            var we = new WorkoutExercise
            {
                Id = Guid.NewGuid(),
                WorkoutId = workoutId,
                ExerciseId = dto.ExerciseId,
                Order = order
            };

            _context.WorkoutExercises.Add(we);
            await _context.SaveChangesAsync();

            return Ok(new { we.Id, we.Order });
        }

        // =========================
        // REORDER
        // =========================
        [HttpPut("{workoutId}/reorder")]
        public async Task<IActionResult> Reorder(Guid workoutId, ReorderWorkoutExercisesDto dto)
        {
            var userId = GetUserId();

            var exercises = await _context.WorkoutExercises
                .Include(x => x.Workout)
                .Where(x => x.WorkoutId == workoutId && x.Workout.UserId == userId)
                .ToListAsync();

            int order = 1;

            foreach (var item in dto.Items.OrderBy(x => x.Order))
            {
                var ex = exercises.FirstOrDefault(x => x.Id == item.WorkoutExerciseId);
                if (ex != null)
                    ex.Order = order++;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // =========================
        // ADD SET
        // =========================
        [HttpPost("exercises/{workoutExerciseId}/sets")]
        public async Task<IActionResult> AddSet(Guid workoutExerciseId, AddSetDto dto)
        {
            var userId = GetUserId();

            var we = await _context.WorkoutExercises
                .Include(x => x.Workout)
                .FirstOrDefaultAsync(x => x.Id == workoutExerciseId);

            if (we == null || we.Workout.UserId != userId)
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
                Weight = (decimal)set.Weight!
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

        // PUT: /api/workouts/sets/{setId}
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
            var set = await _context.WorkoutSets.FindAsync(setId);
            if (set == null) return NotFound();

            _context.WorkoutSets.Remove(set);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /api/workouts/{id}
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
        // DELETE EXERCISE FROM WORKOUT
        // =========================
        [HttpDelete("exercises/{workoutExerciseId}")]
        public async Task<IActionResult> DeleteExercise(Guid workoutExerciseId)
        {
            var userId = GetUserId();

            var workoutExercise = await _context.WorkoutExercises
                .Include(x => x.Workout)
                .FirstOrDefaultAsync(x => x.Id == workoutExerciseId);

            if (workoutExercise == null || workoutExercise.Workout.UserId != userId)
                return NotFound();

            _context.WorkoutExercises.Remove(workoutExercise);

            // 🔥 OPTIONAL: reorder after delete (recommended)
            var remaining = await _context.WorkoutExercises
                .Where(x => x.WorkoutId == workoutExercise.WorkoutId)
                .OrderBy(x => x.Order)
                .ToListAsync();

            int order = 1;
            foreach (var ex in remaining)
            {
                ex.Order = order++;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: /api/workouts/filter?days=7
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
