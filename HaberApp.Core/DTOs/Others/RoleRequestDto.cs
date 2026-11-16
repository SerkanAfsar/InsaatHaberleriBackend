using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Others
{
    public sealed record RoleRequestDto(string RoleName, List<string> Claims) : BaseRequestDto();


}
