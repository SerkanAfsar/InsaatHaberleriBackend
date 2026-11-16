using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.AppUser
{
    public sealed record AppUserResponseDto(
        string Name,
        string Surname,
        string? UserId = null,
        string? Email = null,
        string? PictureUrl = null,
        List<string> Roles = null
        ) : BaseResponseDto();
}
