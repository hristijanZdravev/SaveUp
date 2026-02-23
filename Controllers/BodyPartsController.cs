using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeakLift.Services;

namespace PeakLift.Controllers
{
    [ApiController]
    [Route("api/body-parts")]
    [Authorize]
    public class BodyPartsController : ControllerBase
    {
        private readonly IBodyPartsService _bodyPartsService;

        public BodyPartsController(IBodyPartsService bodyPartsService)
        {
            _bodyPartsService = bodyPartsService;
        }

        private string GetUserId() =>
            User?.Identity?.Name
            ?? throw new UnauthorizedAccessException();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bodyParts = await _bodyPartsService.GetAllAsync();
            return Ok(bodyParts);
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetStats(Guid id)
        {
            var userId = GetUserId();
            var dto = await _bodyPartsService.GetStatsAsync(userId, id);
            return Ok(dto);
        }
    }
}
