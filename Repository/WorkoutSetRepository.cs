using Microsoft.EntityFrameworkCore;
using PeakLift.Data;
using PeakLift.Models;

namespace PeakLift.Repository
{
    public class WorkoutSetRepository : IWorkoutSetRepository
    {
        private readonly Context _context;

        public WorkoutSetRepository(Context context)
        {
            _context = context;
        }

        public Task<int> CountByUserAndBodyGroupAsync(string userId, Guid bodyGroupId, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutSets
                .Where(s =>
                    s.WorkoutExercise.Workout.UserId == userId &&
                    s.WorkoutExercise.Exercise.BodyGroupId == bodyGroupId)
                .CountAsync(cancellationToken);
        }

        public Task<int> CountByWorkoutIdsAsync(List<Guid> workoutIds, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutSets
                .Where(s => workoutIds.Contains(s.WorkoutExercise.WorkoutId))
                .CountAsync(cancellationToken);
        }

        public Task<int> SumRepsByWorkoutIdsAsync(List<Guid> workoutIds, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutSets
                .Where(s => workoutIds.Contains(s.WorkoutExercise.WorkoutId))
                .SumAsync(s => s.Reps, cancellationToken);
        }

        public async Task<double> AverageRepsByWorkoutIdsAsync(List<Guid> workoutIds, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutSets
                .Where(s => workoutIds.Contains(s.WorkoutExercise.WorkoutId))
                .AverageAsync(s => (double?)s.Reps, cancellationToken) ?? 0;
        }

        public Task<WorkoutSet?> GetByIdWithOwnershipAsync(Guid setId, string userId, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutSets
                .Include(s => s.WorkoutExercise)
                .ThenInclude(we => we.Workout)
                .FirstOrDefaultAsync(s => s.Id == setId && s.WorkoutExercise.Workout.UserId == userId, cancellationToken);
        }

        public Task<WorkoutSet?> FindByIdAsync(Guid setId, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutSets
                .FirstOrDefaultAsync(s => s.Id == setId, cancellationToken);
        }

        public Task AddAsync(WorkoutSet workoutSet, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutSets.AddAsync(workoutSet, cancellationToken).AsTask();
        }

        public void Remove(WorkoutSet workoutSet)
        {
            _context.WorkoutSets.Remove(workoutSet);
        }

        public IQueryable<WorkoutSet> Query()
        {
            return _context.WorkoutSets;
        }
    }
}
