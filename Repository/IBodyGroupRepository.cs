using PeakLift.Models;

namespace PeakLift.Repository
{
    public interface IBodyGroupRepository
    {
        Task<List<BodyGroup>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
