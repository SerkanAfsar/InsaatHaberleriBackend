using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Category
{


    public record CategoryResponseDto(int? Id,
        string CategoryName,
        string SeoTitle,
        string SeoDescription,
        int Queue
        ) : BaseResponseDto(Id);
}
