using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Others
{
    public sealed record GetAllDataTypeRequestDto : BaseRequestDto
    {
        public int Offset { get; set; } = 0;
        public int Count { get; set; } = 10;
        public string? GlobalFilter { get; set; } = null;
        public string? Sorting { get; set; } = null;
        public string? Filters { get; set; } = null;
        public bool DateSort { get; set; } = false;
        public bool? IsEnabled { get; set; } = null!;
        public int? CategoryId { get; set; } = null!;
    }
}
