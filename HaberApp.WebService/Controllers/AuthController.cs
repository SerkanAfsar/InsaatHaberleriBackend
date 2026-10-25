using HaberApp.Core.DTOs.AppUser;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Services;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<LoginRequestDto, TokenResponseDto>))]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authenticationService;

        public AuthController(IAuthService authenticationService)
        {
            this.authenticationService = authenticationService;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto, CancellationToken cancellationToken)
        {
            var result = await this.authenticationService.CreateTokenAsync(loginDto, cancellationToken);
            return Ok(result);
        }
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken = default)
        {
            var result = await this.authenticationService.CreateAccessTokenByRefreshTokenAysnc(refreshToken, cancellationToken);
            return Ok(result);
        }
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken = default)
        {
            var result = await this.authenticationService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
            return Ok(result);
        }
        [HttpGet("me")]
        public async Task<IActionResult> AuthMe(CancellationToken cancellationToken)
        {
            var result = await this.authenticationService.AuthMeAsync(cancellationToken);
            return Ok(result);
        }
        //[HttpPost("Confirm")]
        //public async Task<IActionResult> Confirm([FromBody] ConfirmRequestDto confirmRequestDto, CancellationToken cancellationToken = default)
        //{
        //    var result = await this.authenticationService.ConfirmMailAsync(confirmRequestDto.AppUserId, confirmRequestDto.Token, cancellationToken); ;
        //    return Ok(result);
        //}
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] AppUserRequestDto createUserData, CancellationToken cancellationToken = default)
        {
            var result = await this.authenticationService.CreateUserProcessAsync(createUserData, cancellationToken); ;
            return Ok(result);
        }
    }
}
