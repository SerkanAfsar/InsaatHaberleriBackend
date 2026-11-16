using HaberApp.Core.DTOs.Base;
using HaberApp.Core.Enums;

namespace HaberApp.Core.DTOs.CategoryNewsUrl
{

    //public string Url { get; set; } = null!;
    //public int CategoryId { get; set; }
    //public Category Category { get; set; }
    //public NewsSites NewsSite { get; set; }
    public sealed record CategoryNewsUrlRequestDto(int? id,
        string Url,
        int CategoryId,
        NewsSites newsSite
        ) : BaseRequestDto();


}
