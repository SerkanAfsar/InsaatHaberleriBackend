using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.Services;

namespace HaberApp.Services.Services
{
    public class ArticleService : ServiceBase<ArticleRequestDto, Article, ArticleResponseDto>, IArticlesService
    {
    }
}
