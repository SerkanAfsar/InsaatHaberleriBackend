using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Category
{
    public record CategoryRequestDto(int? id, string CategoryName,
        string SeoTitle,
        string SeoDescription,
        int? Queue
        ) : BaseRequestDto();


}
