using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeakLift.Services;

namespace PeakLift.Controllers
{
    [ApiController]
    [Route("api/exercises")]
    [Authorize]
    public class ExercisesController : ControllerBase
    {
        private readonly IExercisesService _exercisesService;

        public ExercisesController(IExercisesService exercisesService)
        {
            _exercisesService = exercisesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _exercisesService.GetAllAsync();
            return Ok(exercises);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var exercise = await _exercisesService.GetByIdAsync(id);
            return exercise == null ? NotFound() : Ok(exercise);
        }

        [HttpGet("by-body-part/{bodyPart}")]
        public async Task<IActionResult> GetByBodyPart(string bodyPart)
        {
            var exercises = await _exercisesService.GetByBodyPartAsync(bodyPart);
            return Ok(exercises);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var exercises = await _exercisesService.SearchAsync(query);
            return Ok(exercises);
        }
    }
}
