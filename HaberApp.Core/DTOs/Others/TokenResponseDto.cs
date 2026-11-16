using HaberApp.Core.DTOs.Base;

namespace HaberApp.Core.DTOs.Others
{
    public sealed record TokenResponseDto(string? AccessToken,
          DateTime? AccessTokenExpiration,
          string? RefreshToken,
          DateTime? RefreshTokenExpiration) : BaseResponseDto()
    {
        public static TokenResponseDto CreateNull() => new TokenResponseDto(null, null, null, null);
    }
}
