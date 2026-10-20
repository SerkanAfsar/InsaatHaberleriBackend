using HaberApp.Core.DTOs.Base;
using HaberApp.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace HaberApp.Core.DTOs.Article
{
    public record ArticleRequestDto(string Title,
        string SubTitle,
        string Content,
        IFormFile ImageFile,
        string SeoTitle,
        string SeoDescription,
        int CategoryId,
        NewsSites newsSite
        ) : BaseRequestDto();

}
