using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeakLift.Services;

namespace PeakLift.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        private string GetUserId() =>
            User?.Identity?.Name ?? throw new UnauthorizedAccessException();

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] int days = 7)
        {
            var userId = GetUserId();
            var summary = await _analyticsService.GetSummaryAsync(userId, days);
            return Ok(summary);
        }

        [HttpGet("sets-per-day")]
        public async Task<IActionResult> SetsPerDay([FromQuery] int days = 7)
        {
            var userId = GetUserId();
            var data = await _analyticsService.GetSetsPerDayAsync(userId, days);
            return Ok(data);
        }

        [HttpGet("body-part-distribution")]
        public async Task<IActionResult> BodyPartDistribution([FromQuery] int days = 7)
        {
            var userId = GetUserId();
            var data = await _analyticsService.GetBodyPartDistributionAsync(userId, days);
            return Ok(data);
        }
    }
}
