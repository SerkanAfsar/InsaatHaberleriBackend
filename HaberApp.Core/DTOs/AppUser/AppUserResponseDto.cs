using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.AppUser
{
    public record AppUserResponseDto(
        string Name,
        string Surname,
        string? UserId = null,
        string? Email = null,
        string? PictureUrl = null) : BaseResponseDto();
}
