using Microsoft.EntityFrameworkCore;
using PeakLift.Data;
using PeakLift.Models;

namespace PeakLift.Repository
{
    public class BodyGroupRepository : IBodyGroupRepository
    {
        private readonly Context _context;

        public BodyGroupRepository(Context context)
        {
            _context = context;
        }

        public Task<List<BodyGroup>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _context.BodyGroups
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
