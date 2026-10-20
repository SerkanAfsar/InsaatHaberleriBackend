using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Base;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Utils;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace HaberApp.Core.Services
{
    public interface IServiceBase<TRequestDto, TEntity, TResponseDto>
        where TRequestDto : BaseRequestDto
        where TEntity : BaseEntity
        where TResponseDto : BaseResponseDto
    {
        Task<ResponseResult<TResponseDto>> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ResponseResult<TResponseDto>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default);
        Task<ResponseResult<TResponseDto>> CreateAsync(TRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<ResponseResult<TResponseDto>> UpdateAsync(int id, TRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<ResponseResult<TResponseDto>> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<ResponseResult<TResponseDto>> GetAllByQueryAsync(GetAllDataTypeRequestDto requestDto, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default);

    }
}
