using HaberApp.Core.DTOs.Base;
using HaberApp.Core.DTOs.Category;

namespace HaberApp.Core.DTOs.Article
{
    public sealed record ArticleResponseDto(int? Id,
        string Title,
        string SubDescription,
        string Content,
        string SeoTitle,
        string SeoDescription,
        string? ImageId,
        int CategoryId,
        int ReadedCount,
        CategoryResponseDto category,
        string? SourceUrl = null
        ) : BaseResponseDto(Id);
}
