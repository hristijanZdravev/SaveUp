using Microsoft.EntityFrameworkCore;
using PeakLift.Data;
using PeakLift.Models;

namespace PeakLift.Repository
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly Context _context;

        public WorkoutRepository(Context context)
        {
            _context = context;
        }

        public Task<int> CountByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return _context.Workouts
                .Where(w => w.UserId == userId)
                .CountAsync(cancellationToken);
        }

        public Task<List<Workout>> GetPagedByUserAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public Task<Workout?> GetByIdForUserAsync(Guid id, string userId, CancellationToken cancellationToken = default)
        {
            return _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, cancellationToken);
        }

        public Task<Workout?> GetWithGraphForUserAsync(Guid id, string userId, CancellationToken cancellationToken = default)
        {
            return _context.Workouts
                .AsNoTracking()
                .Include(w => w.WorkoutExercises.OrderBy(we => we.Order))
                .ThenInclude(we => we.Exercise)
                .Include(w => w.WorkoutExercises.OrderBy(we => we.Order))
                .ThenInclude(we => we.Sets.OrderBy(s => s.SetNumber))
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, cancellationToken);
        }

        public Task AddAsync(Workout workout, CancellationToken cancellationToken = default)
        {
            return _context.Workouts.AddAsync(workout, cancellationToken).AsTask();
        }

        public void Remove(Workout workout)
        {
            _context.Workouts.Remove(workout);
        }

        public Task<List<Workout>> FilterByDaysAsync(string userId, DateTime fromDate, CancellationToken cancellationToken = default)
        {
            return _context.Workouts
                .Where(w => w.UserId == userId && w.Date >= fromDate)
                .OrderByDescending(w => w.Date)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public Task<List<Workout>> GetRecentByUserAsync(string userId, int limit, CancellationToken cancellationToken = default)
        {
            return _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public IQueryable<Workout> QueryByUser(string userId)
        {
            return _context.Workouts.Where(w => w.UserId == userId);
        }
    }
}
