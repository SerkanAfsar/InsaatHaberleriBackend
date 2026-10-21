using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Article
{
    public record ArticleResponseDto(int? Id) : BaseResponseDto(Id);
}
