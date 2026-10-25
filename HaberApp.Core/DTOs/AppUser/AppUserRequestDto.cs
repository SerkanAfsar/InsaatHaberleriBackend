using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.AppUser
{
    public record AppUserRequestDto(
        string EMail,
        string Password,
        string Name,
        string Surname,
        int CityId,
        int DistrictId,
        List<string> RoleIds,
        string? PictureUrl = null
        ) : BaseRequestDto()
    {

    }
}
