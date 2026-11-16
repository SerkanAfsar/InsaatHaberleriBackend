using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.AppUser;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Services;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<AppUserRequestDto, AppUserResponseDto>))]
    [CustomAuthorizationFilter<BaseEntity>(true)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId, CancellationToken cancellationToken = default)
        {
            var result = await this.userService.FindUserByIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("Paginated")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDataTypeRequestDto getAllDataTypeRequestDto, CancellationToken cancellationToken = default)
        {

            var result = await this.userService.GetAllUsersByFilterAsync(getAllDataTypeRequestDto, cancellationToken);
            return Ok(result);
        }
        [HttpPost]

        public async Task<IActionResult> Post([FromBody] AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.userService.CreateUserProcessAsync(appUserRequestDto, cancellationToken);
            return Ok(result);
        }

        [HttpPost("RegisterUser")]
        //[CustomAuthorizationFilter(Permissions.Users.Create)]
        public async Task<IActionResult> RegisterUser([FromBody] AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.userService.CreateMemberProcessAsync(appUserRequestDto, cancellationToken);
            return Ok(result);
        }
        [HttpPut("{userId}")]

        public async Task<IActionResult> Update(string userId, [FromBody] AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.userService.UpdateUserProcessAsync(userId, appUserRequestDto, cancellationToken);
            return Ok(result);
        }
        [HttpDelete("{userId}")]

        public async Task<IActionResult> Delete(string userId, CancellationToken cancellationToken = default)
        {
            var result = await this.userService.DeleteUserProcessAsync(userId, cancellationToken);
            return Ok(result);
        }
        [HttpPost("UpdatePassword")]

        [ServiceFilter(typeof(CustomFilterAttribute<UpdatePasswordRequestDto, AppUserResponseDto>))]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequestDto updatePasswordRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.userService.UpdateUserPasswordProcessAsync(updatePasswordRequestDto, cancellationToken);
            return Ok(result);
        }
    }
}
