using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Others
{
    public record LoginRequestDto(string EMail, string Password) : BaseRequestDto();
}
