using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeakLift.Services;

namespace PeakLift.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private string GetUserId() =>
            User?.Identity?.Name ?? throw new UnauthorizedAccessException();

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats([FromQuery] int days = 7)
        {
            var userId = GetUserId();
            var stats = await _dashboardService.GetStatsAsync(userId, days);
            return Ok(stats);
        }

        [HttpGet("recent-workouts")]
        public async Task<IActionResult> GetRecent([FromQuery] int limit = 5)
        {
            var userId = GetUserId();
            var workouts = await _dashboardService.GetRecentAsync(userId, limit);
            return Ok(workouts);
        }
    }
}
