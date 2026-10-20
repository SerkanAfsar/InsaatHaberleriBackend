namespace HaberApp.Core.UnitOfWorks
{
    public interface IUnitOfWork
    {
        void Commit();
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
