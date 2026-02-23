using PeakLift.Dtos;

namespace PeakLift.Services
{
    public interface IWorkoutsService
    {
        Task<WorkoutPagedResultDto> GetAllAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<WorkoutDetailsDto?> GetByIdAsync(string userId, Guid id, CancellationToken cancellationToken = default);
        Task<WorkoutListDto> CreateAsync(string userId, WorkoutCreateDto dto, CancellationToken cancellationToken = default);
        Task<AddExerciseResultDto?> AddExerciseAsync(string userId, Guid workoutId, AddExerciseDto dto, CancellationToken cancellationToken = default);
        Task<bool> ReorderAsync(string userId, Guid workoutId, ReorderWorkoutExercisesDto dto, CancellationToken cancellationToken = default);
        Task<WorkoutSetDto?> AddSetAsync(string userId, Guid workoutExerciseId, AddSetDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(string userId, Guid id, WorkoutUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateSetAsync(string userId, Guid setId, UpdateSetDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteSetAsync(Guid setId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string userId, Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeleteExerciseAsync(string userId, Guid workoutExerciseId, CancellationToken cancellationToken = default);
        Task<List<WorkoutListDto>> FilterByDaysAsync(string userId, int days, CancellationToken cancellationToken = default);
    }
}
