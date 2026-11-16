using HaberApp.Core.DTOs.AppUser;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Utils;

namespace HaberApp.Core.Services
{
    public interface IUserService
    {
        Task<ResponseResult<AppUserResponseDto>> FindUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<ResponseResult<AppUserResponseDto>> GetAllUsersByFilterAsync(GetAllDataTypeRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<ResponseResult<AppUserResponseDto>> CreateUserProcessAsync(AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default);
        Task<ResponseResult<AppUserResponseDto>> UpdateUserProcessAsync(string userId, AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default);
        Task<ResponseResult<AppUserResponseDto>> DeleteUserProcessAsync(string userId, CancellationToken cancellationToken = default);
        Task<ResponseResult<AppUserResponseDto>> CreateMemberProcessAsync(AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default);
        Task<ResponseResult<AppUserResponseDto>> UpdateUserPasswordProcessAsync(UpdatePasswordRequestDto passwordRequestDto, CancellationToken cancellationToken = default);
    }
}
