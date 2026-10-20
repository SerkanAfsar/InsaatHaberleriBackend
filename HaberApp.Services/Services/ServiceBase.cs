using AutoMapper;
using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Base;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Exceptions;
using HaberApp.Core.Helpers;
using HaberApp.Core.Repositories;
using HaberApp.Core.Services;
using HaberApp.Core.UnitOfWorks;
using HaberApp.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

namespace HaberApp.Services.Services
{
    public class ServiceBase<TRequestDto, TDomain, TResponseDto> : IServiceBase<TRequestDto, TDomain, TResponseDto>
        where TRequestDto : BaseRequestDto
        where TDomain : BaseEntity
        where TResponseDto : BaseResponseDto
    {
        protected readonly IRepositoryBase<TDomain> repository;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        protected readonly ResponseResult<TResponseDto> responseResult;

        public ServiceBase()
        {
            this.repository = ServiceProviderHelper.GetRequiredService<IRepositoryBase<TDomain>>() ?? throw new Exception("repository hata");
            this.mapper = ServiceProviderHelper.GetRequiredService<IMapper>() ?? throw new Exception("Mapper Hata");
            this.unitOfWork = ServiceProviderHelper.GetRequiredService<IUnitOfWork>() ?? throw new Exception("Unit Of Work Hata");
            this.responseResult = new();
        }

        public virtual async Task<ResponseResult<TResponseDto>> CreateAsync(TRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var result = await this.repository.CreateAsync(this.mapper.Map<TDomain>(requestDto), cancellationToken);
            await this.unitOfWork.CommitAsync(cancellationToken);
            this.responseResult.Entity = this.mapper.Map<TResponseDto>(result);
            return this.responseResult;
        }

        public virtual async Task<ResponseResult<TResponseDto>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var result = await this.repository.DeleteAsync(id, cancellationToken);
            if (result == null)
            {
                throw new CustomException("error", $"{typeof(TDomain).Name} Not Found", HttpStatusCode.NotFound);
            }
            await this.unitOfWork.CommitAsync(cancellationToken);
            this.responseResult.Entity = this.mapper.Map<TResponseDto>(result);
            return this.responseResult;
        }

        public virtual async Task<ResponseResult<TResponseDto>> FindAllAsync(Expression<Func<TDomain, bool>>? predicate, Func<IQueryable<TDomain>, IIncludableQueryable<TDomain, object>>? include = null, CancellationToken cancellationToken = default)
        {
            var data = await this.repository.GetAllAsync(predicate, include).ToListAsync(cancellationToken);
            var result = this.mapper.Map<List<TResponseDto>>(data);
            this.responseResult.Entities = result;
            this.responseResult.RowCount = result.Count();
            return this.responseResult;
        }

        public virtual async Task<ResponseResult<TResponseDto>> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var result = await this.repository.GetByIdAsync(id, cancellationToken);
            if (result == null)
            {
                throw new CustomException("error", $"{typeof(TDomain).Name} Not Found", HttpStatusCode.NotFound);
            }
            this.responseResult.Entity = this.mapper.Map<TResponseDto>(result);
            return this.responseResult;
        }

        public virtual async Task<ResponseResult<TResponseDto>> GetAllByQueryAsync(GetAllDataTypeRequestDto requestDto, Expression<Func<TDomain, bool>>? predicate = null, Func<IQueryable<TDomain>, IIncludableQueryable<TDomain, object>>? include = null, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var result = this.repository.GetAllAsync(null, null, cancellationToken).OfType<TDomain>();

            if (include != null)
            {
                result = include(result);
            }
            if (predicate != null)
            {
                result = result.Where(predicate);
            }
            if (requestDto.DateSort)
            {
                result = result.OrderByDescending(a => a.CreatedAt);
            }
            if (!string.IsNullOrEmpty(requestDto.Sorting))
            {
                var sorting = JsonSerializer.Deserialize<List<SortingDto>>(requestDto.Sorting);

                if (sorting?.Count > 0)
                {
                    var sort = sorting.First();

                    result = result.OrderBy($"{sort.id} {(sort.desc ? "desc" : "")}");
                }
            }
            var lastResult = await result.Skip(requestDto.Offset).Take(requestDto.Count).ToListAsync(cancellationToken);
            this.responseResult.Entities = this.mapper.Map<List<TResponseDto>>(lastResult);
            this.responseResult.RowCount = await result.CountAsync(cancellationToken);
            return this.responseResult;
        }

        public virtual async Task<ResponseResult<TResponseDto>> UpdateAsync(int id, TRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var updatedMapper = this.mapper.Map<TDomain>(requestDto);

            var result = await this.repository.UpdateAsync(id, updatedMapper, cancellationToken);
            if (result == null)
            {
                throw new CustomException("error", $"{typeof(TDomain).Name} Not Found", HttpStatusCode.NotFound);
            }
            await this.unitOfWork.CommitAsync(cancellationToken);
            var updatedEntity = await this.repository.GetByIdAsync(id, cancellationToken);
            this.responseResult.Entity = this.mapper.Map<TResponseDto>(updatedEntity);
            return this.responseResult;
        }
    }
}
