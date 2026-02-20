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
    }
}
