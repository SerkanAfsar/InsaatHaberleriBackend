using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Others
{
    public sealed record LoginRequestDto(string EMail, string Password, string Token) : BaseRequestDto();
}
