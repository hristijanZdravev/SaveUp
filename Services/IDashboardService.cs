using PeakLift.DTOs;

namespace PeakLift.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync(string userId, int days, CancellationToken cancellationToken = default);
        Task<List<RecentWorkoutDto>> GetRecentAsync(string userId, int limit, CancellationToken cancellationToken = default);
    }
}
