using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.AppUser
{
    public sealed record AppUserRequestDto(
        string EMail,
        string Password,
        string Name,
        string Surname,
        List<string> Roles,
        string? PictureUrl = null
        ) : BaseRequestDto()
    {

    }
}
