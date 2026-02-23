using PeakLift.DTOs;

namespace PeakLift.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsSummaryDto> GetSummaryAsync(string userId, int days, CancellationToken cancellationToken = default);
        Task<List<SetsPerDayDto>> GetSetsPerDayAsync(string userId, int days, CancellationToken cancellationToken = default);
        Task<List<BodyPartDistributionDto>> GetBodyPartDistributionAsync(string userId, int days, CancellationToken cancellationToken = default);
    }
}
