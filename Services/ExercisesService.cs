using PeakLift.DTOs;
using PeakLift.Repository;

namespace PeakLift.Services
{
    public class ExercisesService : IExercisesService
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly CloudinaryService _cloudinaryService;

        public ExercisesService(IExerciseRepository exerciseRepository, CloudinaryService cloudinaryService)
        {
            _exerciseRepository = exerciseRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<List<ExerciseListDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var exercises = await _exerciseRepository.GetAllWithBodyGroupAsync(cancellationToken);
            return exercises.Select(MapToExerciseListDto).ToList();
        }

        public async Task<ExerciseListDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var exercise = await _exerciseRepository.GetByIdWithBodyGroupAsync(id, cancellationToken);
            return exercise == null ? null : MapToExerciseListDto(exercise);
        }

        public async Task<List<ExerciseByBodyPartDto>> GetByBodyPartAsync(string bodyPart, CancellationToken cancellationToken = default)
        {
            var bodyPartLower = bodyPart.ToLower();
            var exercises = await _exerciseRepository.GetByBodyPartNameAsync(bodyPartLower, cancellationToken);

            return exercises.Select(e => new ExerciseByBodyPartDto
            {
                Id = e.Id,
                Title = e.Title,
                ImageUrl = string.IsNullOrEmpty(e.ImagePublicId) ? null : _cloudinaryService.GetExerciseImage(e.ImagePublicId),
                BodyGroup = new ExerciseBodyGroupDto
                {
                    Id = e.BodyGroup.Id,
                    Name = e.BodyGroup.Name
                }
            }).ToList();
        }

        public async Task<List<ExerciseSearchDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            query = (query ?? string.Empty).Trim();
            if (query.Length < 2)
            {
                return new List<ExerciseSearchDto>();
            }

            var exercises = await _exerciseRepository.SearchByTitleAsync(query, cancellationToken: cancellationToken);
            return exercises.Select(e => new ExerciseSearchDto
            {
                Id = e.Id,
                Title = e.Title,
                ImageUrl = string.IsNullOrEmpty(e.ImagePublicId) ? null : _cloudinaryService.GetExerciseImage(e.ImagePublicId)
            }).ToList();
        }

        private ExerciseListDto MapToExerciseListDto(PeakLift.Models.Exercise exercise)
        {
            return new ExerciseListDto
            {
                Id = exercise.Id,
                Title = exercise.Title,
                Description = exercise.Description,
                ImageUrl = string.IsNullOrEmpty(exercise.ImagePublicId) ? null : _cloudinaryService.GetExerciseImage(exercise.ImagePublicId),
                BodyGroup = new ExerciseBodyGroupDto
                {
                    Id = exercise.BodyGroup.Id,
                    Name = exercise.BodyGroup.Name
                }
            };
        }
    }
}
