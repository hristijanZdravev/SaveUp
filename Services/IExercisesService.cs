using PeakLift.DTOs;

namespace PeakLift.Services
{
    public interface IExercisesService
    {
        Task<List<ExerciseListDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ExerciseListDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ExerciseByBodyPartDto>> GetByBodyPartAsync(string bodyPart, CancellationToken cancellationToken = default);
        Task<List<ExerciseSearchDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
    }
}
