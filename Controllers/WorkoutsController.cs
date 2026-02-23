using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeakLift.Dtos;
using PeakLift.Services;

namespace PeakLift.Controllers
{
    [ApiController]
    [Route("api/workouts")]
    [Authorize]
    public class WorkoutsController : ControllerBase
    {
        private readonly IWorkoutsService _workoutsService;

        public WorkoutsController(IWorkoutsService workoutsService)
        {
            _workoutsService = workoutsService;
        }

        private string GetUserId() =>
            User?.Identity?.Name ?? throw new UnauthorizedAccessException();

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 9)
        {
            var userId = GetUserId();
            var result = await _workoutsService.GetAllAsync(userId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();
            var workout = await _workoutsService.GetByIdAsync(userId, id);
            return workout == null ? NotFound() : Ok(workout);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkoutCreateDto dto)
        {
            var workout = await _workoutsService.CreateAsync(GetUserId(), dto);
            return Ok(workout);
        }

        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> AddExercise(Guid workoutId, AddExerciseDto dto)
        {
            var result = await _workoutsService.AddExerciseAsync(GetUserId(), workoutId, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{workoutId}/reorder")]
        public async Task<IActionResult> Reorder(Guid workoutId, ReorderWorkoutExercisesDto dto)
        {
            var updated = await _workoutsService.ReorderAsync(GetUserId(), workoutId, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpPost("exercises/{workoutExerciseId}/sets")]
        public async Task<IActionResult> AddSet(Guid workoutExerciseId, AddSetDto dto)
        {
            var set = await _workoutsService.AddSetAsync(GetUserId(), workoutExerciseId, dto);
            return set == null ? NotFound() : Ok(set);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] WorkoutUpdateDto dto)
        {
            var updated = await _workoutsService.UpdateAsync(GetUserId(), id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpPut("sets/{setId}")]
        public async Task<IActionResult> UpdateSet(Guid setId, [FromBody] UpdateSetDto dto)
        {
            var updated = await _workoutsService.UpdateSetAsync(GetUserId(), setId, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("sets/{setId}")]
        public async Task<IActionResult> DeleteSet(Guid setId)
        {
            var deleted = await _workoutsService.DeleteSetAsync(setId);
            return deleted ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _workoutsService.DeleteAsync(GetUserId(), id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpDelete("exercises/{workoutExerciseId}")]
        public async Task<IActionResult> DeleteExercise(Guid workoutExerciseId)
        {
            var deleted = await _workoutsService.DeleteExerciseAsync(GetUserId(), workoutExerciseId);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterByDays([FromQuery] int days = 7)
        {
            if (days <= 0)
            {
                return BadRequest("Days must be greater than 0");
            }

            var workouts = await _workoutsService.FilterByDaysAsync(GetUserId(), days);
            return Ok(workouts);
        }
    }
}

