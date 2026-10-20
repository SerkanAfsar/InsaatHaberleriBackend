using HaberApp.Core.DTOs.AppUser;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Utils;

namespace HaberApp.Core.Services
{
    public interface IAuthService
    {
        Task<ResponseResult<TokenResponseDto>> CreateTokenAsync(LoginRequestDto loginDto, CancellationToken ct = default);
        Task<ResponseResult<TokenResponseDto>> CreateAccessTokenByRefreshTokenAysnc(string refreshToken, CancellationToken ct = default);
        Task<ResponseResult<TokenResponseDto>> RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task<ResponseResult<AppUserResponseDto>> AuthMeAsync(CancellationToken ct = default);
        //Task<ResponseResult<TokenResponseDto>> ConfirmMailAsync(ConfirmRequestDto requestDto, CancellationToken ct = default);
        Task<ResponseResult<AppUserResponseDto>> CreateUserProcessAsync(AppUserRequestDto requestDto, CancellationToken ct = default);
    }
}
