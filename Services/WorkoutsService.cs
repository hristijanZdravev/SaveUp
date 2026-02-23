using PeakLift.Dtos;
using PeakLift.Models;
using PeakLift.Repository;

namespace PeakLift.Services
{
    public class WorkoutsService : IWorkoutsService
    {
        private readonly IWorkoutRepository _workoutRepository;
        private readonly IWorkoutExerciseRepository _workoutExerciseRepository;
        private readonly IWorkoutSetRepository _workoutSetRepository;
        private readonly IAppUnitOfWork _unitOfWork;

        public WorkoutsService(
            IWorkoutRepository workoutRepository,
            IWorkoutExerciseRepository workoutExerciseRepository,
            IWorkoutSetRepository workoutSetRepository,
            IAppUnitOfWork unitOfWork)
        {
            _workoutRepository = workoutRepository;
            _workoutExerciseRepository = workoutExerciseRepository;
            _workoutSetRepository = workoutSetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkoutPagedResultDto> GetAllAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var total = await _workoutRepository.CountByUserAsync(userId, cancellationToken);
            var workouts = await _workoutRepository.GetPagedByUserAsync(userId, page, pageSize, cancellationToken);

            return new WorkoutPagedResultDto
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Data = workouts.Select(w => new WorkoutListDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    Date = w.Date
                }).ToList()
            };
        }

        public async Task<WorkoutDetailsDto?> GetByIdAsync(string userId, Guid id, CancellationToken cancellationToken = default)
        {
            var workout = await _workoutRepository.GetWithGraphForUserAsync(id, userId, cancellationToken);
            if (workout == null)
            {
                return null;
            }

            return new WorkoutDetailsDto
            {
                Id = workout.Id,
                Title = workout.Title,
                Date = workout.Date,
                Exercises = workout.WorkoutExercises
                    .OrderBy(we => we.Order)
                    .Select(we => new WorkoutExerciseDto
                    {
                        Id = we.Id,
                        Order = we.Order,
                        ExerciseId = we.ExerciseId,
                        ExerciseTitle = we.Exercise.Title,
                        Sets = we.Sets
                            .OrderBy(s => s.SetNumber)
                            .Select(s => new WorkoutSetDto
                            {
                                Id = s.Id,
                                SetNumber = s.SetNumber,
                                Reps = s.Reps,
                                Weight = (decimal)s.Weight!
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

        public async Task<WorkoutListDto> CreateAsync(string userId, WorkoutCreateDto dto, CancellationToken cancellationToken = default)
        {
            var workout = new Workout
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = dto.Title,
                Date = dto.Date
            };

            await _workoutRepository.AddAsync(workout, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new WorkoutListDto
            {
                Id = workout.Id,
                Title = workout.Title,
                Date = workout.Date
            };
        }

        public async Task<AddExerciseResultDto?> AddExerciseAsync(string userId, Guid workoutId, AddExerciseDto dto, CancellationToken cancellationToken = default)
        {
            var workout = await _workoutRepository.GetByIdForUserAsync(workoutId, userId, cancellationToken);
            if (workout == null)
            {
                return null;
            }

            var order = await _workoutExerciseRepository.CountByWorkoutIdAsync(workoutId, cancellationToken) + 1;

            var workoutExercise = new WorkoutExercise
            {
                Id = Guid.NewGuid(),
                WorkoutId = workoutId,
                ExerciseId = dto.ExerciseId,
                Order = order
            };

            await _workoutExerciseRepository.AddAsync(workoutExercise, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AddExerciseResultDto
            {
                Id = workoutExercise.Id,
                Order = workoutExercise.Order
            };
        }

        public async Task<bool> ReorderAsync(string userId, Guid workoutId, ReorderWorkoutExercisesDto dto, CancellationToken cancellationToken = default)
        {
            var workout = await _workoutRepository.GetByIdForUserAsync(workoutId, userId, cancellationToken);
            if (workout == null)
            {
                return false;
            }

            var exercises = await _workoutExerciseRepository.GetByWorkoutForUserAsync(workoutId, userId, cancellationToken);

            int order = 1;
            foreach (var item in dto.Items.OrderBy(x => x.Order))
            {
                var exercise = exercises.FirstOrDefault(x => x.Id == item.WorkoutExerciseId);
                if (exercise != null)
                {
                    exercise.Order = order++;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<WorkoutSetDto?> AddSetAsync(string userId, Guid workoutExerciseId, AddSetDto dto, CancellationToken cancellationToken = default)
        {
            var workoutExercise = await _workoutExerciseRepository.GetByIdWithWorkoutAsync(workoutExerciseId, cancellationToken);
            if (workoutExercise == null || workoutExercise.Workout.UserId != userId)
            {
                return null;
            }

            var set = new WorkoutSet
            {
                Id = Guid.NewGuid(),
                WorkoutExerciseId = workoutExerciseId,
                SetNumber = dto.SetNumber,
                Reps = dto.Reps,
                Weight = dto.Weight
            };

            await _workoutSetRepository.AddAsync(set, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new WorkoutSetDto
            {
                Id = set.Id,
                SetNumber = set.SetNumber,
                Reps = set.Reps,
                Weight = (decimal)set.Weight!
            };
        }

        public async Task<bool> UpdateAsync(string userId, Guid id, WorkoutUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var workout = await _workoutRepository.GetByIdForUserAsync(id, userId, cancellationToken);
            if (workout == null)
            {
                return false;
            }

            workout.Title = dto.Title;
            workout.Date = dto.Date;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateSetAsync(string userId, Guid setId, UpdateSetDto dto, CancellationToken cancellationToken = default)
        {
            var set = await _workoutSetRepository.GetByIdWithOwnershipAsync(setId, userId, cancellationToken);
            if (set == null)
            {
                return false;
            }

            set.Reps = dto.Reps;
            set.Weight = dto.Weight;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteSetAsync(Guid setId, CancellationToken cancellationToken = default)
        {
            var set = await _workoutSetRepository.FindByIdAsync(setId, cancellationToken);
            if (set == null)
            {
                return false;
            }

            _workoutSetRepository.Remove(set);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, Guid id, CancellationToken cancellationToken = default)
        {
            var workout = await _workoutRepository.GetByIdForUserAsync(id, userId, cancellationToken);
            if (workout == null)
            {
                return false;
            }

            _workoutRepository.Remove(workout);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteExerciseAsync(string userId, Guid workoutExerciseId, CancellationToken cancellationToken = default)
        {
            var workoutExercise = await _workoutExerciseRepository.GetByIdWithWorkoutAsync(workoutExerciseId, cancellationToken);
            if (workoutExercise == null || workoutExercise.Workout.UserId != userId)
            {
                return false;
            }

            _workoutExerciseRepository.Remove(workoutExercise);

            var remaining = await _workoutExerciseRepository.GetByWorkoutIdOrderedAsync(workoutExercise.WorkoutId, cancellationToken);
            int order = 1;
            foreach (var exercise in remaining)
            {
                exercise.Order = order++;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<WorkoutListDto>> FilterByDaysAsync(string userId, int days, CancellationToken cancellationToken = default)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var workouts = await _workoutRepository.FilterByDaysAsync(userId, startDate, cancellationToken);
            return workouts.Select(w => new WorkoutListDto
            {
                Id = w.Id,
                Title = w.Title,
                Date = w.Date
            }).ToList();
        }
    }
}
