using HaberApp.Core.Domains;
using HaberApp.Core.Repositories;
using HaberApp.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace HaberApp.DataAccess.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T>
        where T : BaseEntity
    {
        protected readonly DbSet<T> dbSet;
        protected readonly AppDbContext appDbContext;

        public RepositoryBase(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
            this.dbSet = this.appDbContext.Set<T>();
        }

        public virtual async Task<IQueryable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await this.dbSet.AddRangeAsync(entities, cancellationToken);
            return entities.AsQueryable();
        }

        public virtual async Task<T?> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await this.dbSet.AddAsync(entity, cancellationToken);
            return entity;

        }

        public virtual async Task<T?> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await this.dbSet.FindAsync(id);
            this.dbSet.Remove(entity!);
            return entity ?? null;
        }



        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await this.dbSet.FindAsync(id, cancellationToken);
            return result ?? null;
        }

        public virtual async Task<IQueryable<T>> RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => this.dbSet.RemoveRange(entities));
            return entities.AsQueryable();
        }

        public virtual Task<int> TotalCount()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T?> UpdateAsync(int id, T entity, CancellationToken cancellationToken = default)
        {
            var existedEntity = await this.dbSet.FindAsync(id, cancellationToken);

            if (existedEntity != null)
            {
                entity.Id = id;
                await Task.Run(() => appDbContext.Entry(existedEntity).CurrentValues.SetValues(entity), cancellationToken);
            }

            return existedEntity != null ? entity : null;
        }

        public virtual async Task<T?> FindFirstByFilterAsyc(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.dbSet.Where(predicate).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }

        public virtual IQueryable<T> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();

            var query = this.dbSet.AsNoTracking().AsQueryable();

            if (include != null)
                query = include(query);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }
    }
}
