using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Message
{
    public sealed record MessageResponseDto(int? Id, string NameSurname, string Email, string Subject, string MessageText, string IsReaded) : BaseResponseDto(Id);
}
