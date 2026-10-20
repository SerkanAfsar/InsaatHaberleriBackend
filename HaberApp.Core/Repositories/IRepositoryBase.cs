using HaberApp.Core.Domains;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace HaberApp.Core.Repositories
{
    public interface IRepositoryBase<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        IQueryable<T> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, CancellationToken cancellationToken = default);
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
        Task<T?> UpdateAsync(int id, T entity, CancellationToken cancellationToken = default);
        Task<T?> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IQueryable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<IQueryable<T>> RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<int> TotalCount();
        Task<T?> FindFirstByFilterAsyc(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
