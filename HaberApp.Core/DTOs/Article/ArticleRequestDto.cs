using HaberApp.Core.DTOs.Base;
using HaberApp.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace HaberApp.Core.DTOs.Article
{
    public record ArticleRequestDto(string Title,
        string SubDescription,
        string Content,
        IFormFile ImageFile,
        string SeoTitle,
        string SeoDescription,
        int CategoryId,
        NewsSites newsSite,
        string? ImageId = null
        ) : BaseRequestDto();

}
