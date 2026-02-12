using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveUp.Data;
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
            User?.Identity?.Name
            ?? throw new UnauthorizedAccessException();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .ToListAsync();

            return Ok(workouts);
        }

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

            if (workout == null) return NotFound();

            return Ok(workout);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Workout workout)
        {
            workout.UserId = GetUserId();
            workout.Id = Guid.NewGuid();

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Workout updated)
        {
            var userId = GetUserId();

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null) return NotFound();

            workout.Title = updated.Title;
            workout.Date = updated.Date;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null) return NotFound();

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> AddExercise(Guid workoutId, [FromBody] Guid exerciseId)
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
                ExerciseId = exerciseId,
                Order = await _context.WorkoutExercises
                            .Where(x => x.WorkoutId == workoutId)
                            .CountAsync() + 1
            };

            _context.WorkoutExercises.Add(workoutExercise);
            await _context.SaveChangesAsync();

            return Ok(workoutExercise);
        }

        [HttpPost("exercises/{workoutExerciseId}/sets")]
        public async Task<IActionResult> AddSet(Guid workoutExerciseId, [FromBody] WorkoutSet set)
        {
            var userId = GetUserId();

            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.Id == workoutExerciseId);

            if (workoutExercise == null || workoutExercise.Workout.UserId != userId)
                return NotFound();

            set.Id = Guid.NewGuid();
            set.WorkoutExerciseId = workoutExerciseId;

            _context.WorkoutSets.Add(set);
            await _context.SaveChangesAsync();

            return Ok(set);
        }

        [HttpPut("sets/{setId}")]
        public async Task<IActionResult> UpdateSet(Guid setId, [FromBody] WorkoutSet updated)
        {
            var userId = GetUserId();

            var set = await _context.WorkoutSets
                .Include(s => s.WorkoutExercise)
                .ThenInclude(we => we.Workout)
                .FirstOrDefaultAsync(s => s.Id == setId);

            if (set == null || set.WorkoutExercise.Workout.UserId != userId)
                return NotFound();

            set.Reps = updated.Reps;
            set.Weight = updated.Weight;

            await _context.SaveChangesAsync();

            return NoContent();
        }

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

    }


}
