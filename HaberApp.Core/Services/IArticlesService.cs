using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Utils;

namespace HaberApp.Core.Services
{
    public interface IArticlesService : IServiceBase<ArticleRequestDto, Article, ArticleResponseDto>
    {
        Task<ResponseResult<ArticleResponseDto>> GetLastNewsAsync(GetAllDataTypeRequestDto request, int? categoryId = null, CancellationToken cancellationToken = default);
        Task<ResponseResult<ArticleResponseDto>> GetPopularPosts(int? categoryId = null, CancellationToken token = default);
        Task<ResponseResult<ArticleResponseDto>> GetMostPopularPost(int? categoryId = null, CancellationToken cancellationToken = default);
        Task<ResponseResult<ArticleResponseDto>> IncreaseReadCount(int id, CancellationToken cancellationToken = default);

    }
}
