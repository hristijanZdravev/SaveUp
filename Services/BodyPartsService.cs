using PeakLift.DTOs;
using PeakLift.Repository;

namespace PeakLift.Services
{
    public class BodyPartsService : IBodyPartsService
    {
        private readonly IBodyGroupRepository _bodyGroupRepository;
        private readonly IWorkoutSetRepository _workoutSetRepository;

        public BodyPartsService(
            IBodyGroupRepository bodyGroupRepository,
            IWorkoutSetRepository workoutSetRepository)
        {
            _bodyGroupRepository = bodyGroupRepository;
            _workoutSetRepository = workoutSetRepository;
        }

        public async Task<List<BodyPartDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var bodyParts = await _bodyGroupRepository.GetAllAsync(cancellationToken);

            return bodyParts
                .Select(b => new BodyPartDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToList();
        }

        public async Task<BodyPartStatsDto> GetStatsAsync(string userId, Guid bodyPartId, CancellationToken cancellationToken = default)
        {
            var totalSets = await _workoutSetRepository.CountByUserAndBodyGroupAsync(userId, bodyPartId, cancellationToken);
            return new BodyPartStatsDto { TotalSets = totalSets };
        }
    }
}
