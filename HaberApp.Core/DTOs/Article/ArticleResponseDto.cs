using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Article
{
    public record ArticleResponseDto(int? Id,
        string Title,
        string SubDescription,
        string Content,
        string SeoTitle,
        string SeoDescription,
        string? ImageId,
        int CategoryId
        ) : BaseResponseDto(Id);
}
