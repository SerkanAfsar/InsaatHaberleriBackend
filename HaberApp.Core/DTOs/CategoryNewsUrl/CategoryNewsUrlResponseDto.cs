using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.CategoryNewsUrl
{
    public record CategoryNewsUrlResponseDto(
        int? Id,
        string Url,
        int CategoryId,
        HaberApp.Core.Enums.NewsSites NewsSite) : BaseResponseDto(Id);

}
