using Microsoft.EntityFrameworkCore;
using PeakLift.Data;
using PeakLift.Models;

namespace PeakLift.Repository
{
    public class WorkoutExerciseRepository : IWorkoutExerciseRepository
    {
        private readonly Context _context;

        public WorkoutExerciseRepository(Context context)
        {
            _context = context;
        }

        public Task<int> CountByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutExercises
                .Where(x => x.WorkoutId == workoutId)
                .CountAsync(cancellationToken);
        }

        public Task AddAsync(WorkoutExercise workoutExercise, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutExercises.AddAsync(workoutExercise, cancellationToken).AsTask();
        }

        public Task<List<WorkoutExercise>> GetByWorkoutForUserAsync(Guid workoutId, string userId, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutExercises
                .Include(x => x.Workout)
                .Where(x => x.WorkoutId == workoutId && x.Workout.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public Task<WorkoutExercise?> GetByIdWithWorkoutAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutExercises
                .Include(x => x.Workout)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<List<WorkoutExercise>> GetByWorkoutIdOrderedAsync(Guid workoutId, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutExercises
                .Where(x => x.WorkoutId == workoutId)
                .OrderBy(x => x.Order)
                .ToListAsync(cancellationToken);
        }

        public void Remove(WorkoutExercise workoutExercise)
        {
            _context.WorkoutExercises.Remove(workoutExercise);
        }

        public IQueryable<WorkoutExercise> Query()
        {
            return _context.WorkoutExercises;
        }
    }
}
