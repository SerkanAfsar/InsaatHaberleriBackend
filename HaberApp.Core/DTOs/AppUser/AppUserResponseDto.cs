using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.AppUser
{
    public record AppUserResponseDto(string Name, string Surname) : BaseResponseDto();
}
