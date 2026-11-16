using HaberApp.Core.DTOs.Base;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.Enums;

namespace HaberApp.Core.DTOs.CategoryNewsUrl
{
    public sealed record CategoryNewsUrlResponseDto(
        int? Id,
        string Url,
        int CategoryId,
        NewsSites NewsSite,
        CategoryResponseDto category = null
        ) : BaseResponseDto(Id);

}
