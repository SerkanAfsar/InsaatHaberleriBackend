using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Message
{
    public sealed record MessageRequestDto(string NameSurname, string EMail, string Subject, string MessageText, string Token) : BaseRequestDto();
}
