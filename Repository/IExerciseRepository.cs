using PeakLift.Models;

namespace PeakLift.Repository
{
    public interface IExerciseRepository
    {
        Task<List<Exercise>> GetAllWithBodyGroupAsync(CancellationToken cancellationToken = default);
        Task<Exercise?> GetByIdWithBodyGroupAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Exercise>> GetByBodyPartNameAsync(string bodyPartLower, CancellationToken cancellationToken = default);
        Task<List<Exercise>> SearchByTitleAsync(string query, int take = 20, CancellationToken cancellationToken = default);
    }
}
