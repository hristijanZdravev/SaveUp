using Microsoft.EntityFrameworkCore;
using PeakLift.Data;
using PeakLift.Models;

namespace PeakLift.Repository
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly Context _context;

        public ExerciseRepository(Context context)
        {
            _context = context;
        }

        public Task<List<Exercise>> GetAllWithBodyGroupAsync(CancellationToken cancellationToken = default)
        {
            return _context.Exercises
                .AsNoTracking()
                .Include(e => e.BodyGroup)
                .OrderBy(e => e.Title)
                .ToListAsync(cancellationToken);
        }

        public Task<Exercise?> GetByIdWithBodyGroupAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Exercises
                .AsNoTracking()
                .Include(e => e.BodyGroup)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public Task<List<Exercise>> GetByBodyPartNameAsync(string bodyPartLower, CancellationToken cancellationToken = default)
        {
            return _context.Exercises
                .AsNoTracking()
                .Include(e => e.BodyGroup)
                .Where(e => e.BodyGroup.Name.ToLower() == bodyPartLower)
                .OrderBy(e => e.Title)
                .ToListAsync(cancellationToken);
        }

        public Task<List<Exercise>> SearchByTitleAsync(string query, int take = 20, CancellationToken cancellationToken = default)
        {
            return _context.Exercises
                .AsNoTracking()
                .Where(e => EF.Functions.Like(e.Title, $"%{query}%"))
                .OrderBy(e => e.Title)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}
