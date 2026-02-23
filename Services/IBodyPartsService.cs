using PeakLift.DTOs;

namespace PeakLift.Services
{
    public interface IBodyPartsService
    {
        Task<List<BodyPartDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<BodyPartStatsDto> GetStatsAsync(string userId, Guid bodyPartId, CancellationToken cancellationToken = default);
    }
}
