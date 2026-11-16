using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Others
{
    public sealed record RoleResponseDto(string RoleId, string RoleName, List<string>? Claims = null) : BaseResponseDto((int?)null);


}
