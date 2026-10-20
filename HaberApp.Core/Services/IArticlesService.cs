using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;

namespace HaberApp.Core.Services
{
    public interface IArticlesService : IServiceBase<ArticleRequestDto, Article, ArticleResponseDto>
    {
    }
}
