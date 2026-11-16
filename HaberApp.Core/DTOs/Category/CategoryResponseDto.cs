using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Category
{
    public sealed record CategoryResponseDto(int? Id,
        string CategoryName,
        string SeoTitle,
        string SeoDescription,
        int Queue,
        List<ArticleResponseDto>? SubArticles = null!,
        int? NewsCount = null
        ) : BaseResponseDto(Id);
}
