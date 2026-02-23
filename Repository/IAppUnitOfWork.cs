namespace PeakLift.Repository
{
    public interface IAppUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
