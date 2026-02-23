using PeakLift.Data;

namespace PeakLift.Repository
{
    public class AppUnitOfWork : IAppUnitOfWork
    {
        private readonly Context _context;

        public AppUnitOfWork(Context context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
