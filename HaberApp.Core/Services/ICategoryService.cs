using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.Utils;

namespace HaberApp.Core.Services
{
    public interface ICategoryService : IServiceBase<CategoryRequestDto, Category, CategoryResponseDto>
    {
        Task<ResponseResult<CategoryResponseDto>> GetMostReadedWithArticlesAsync(CancellationToken cancellationToken);
        Task<ResponseResult<CategoryResponseDto>> GetCategoryListWithNewsCountAsync(CancellationToken cancellationToken = default);
        Task<ResponseResult<CategoryResponseDto>> GetRandomCategoryWithNews(CancellationToken cancellationToken = default);

    }
}
