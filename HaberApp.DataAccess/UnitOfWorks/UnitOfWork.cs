using HaberApp.Core.UnitOfWorks;
using HaberApp.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace HaberApp.DataAccess.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext appDbContext = null!;
        private IDbContextTransaction dbContextTransaction;

        public UnitOfWork(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            this.dbContextTransaction = await this.appDbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (this.dbContextTransaction != null)
            {
                await this.dbContextTransaction.CommitAsync(cancellationToken);
            }
            else
            {
                await this.appDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (this.dbContextTransaction != null)
            {
                await this.dbContextTransaction.RollbackAsync(cancellationToken);
            }
        }

        public void Commit()
        {
            //if (this.dbContextTransaction != null)
            //{

            //}
            //else
            //{
            //    this.appDbContext.SaveChanges();
            //}
            this.appDbContext.SaveChanges();
        }
    }
}
