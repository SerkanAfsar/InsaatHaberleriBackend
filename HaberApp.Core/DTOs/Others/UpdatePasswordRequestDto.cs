using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Others
{
    public sealed record UpdatePasswordRequestDto(string UserId,
        string OldPassword,
        string NewPassword,
        string NewPasswordConfirm
        ) : BaseRequestDto();

}
