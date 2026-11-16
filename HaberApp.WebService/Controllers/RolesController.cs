using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Services;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<RoleRequestDto, RoleResponseDto>))]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {

            var result = await this.roleService.GetRoleByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await this.roleService.GetAllRolesAsync(cancellationToken);
            return Ok(result);
        }
        [HttpGet("Paginated")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDataTypeRequestDto getAllDataTypeRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.roleService.GetAllRolesAsync(getAllDataTypeRequestDto, cancellationToken);
            return Ok(result);
        }

        [HttpPost]

        public async Task<IActionResult> Post([FromBody] RoleRequestDto roleRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.roleService.CreateRoleAsync(roleRequestDto, cancellationToken);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] RoleRequestDto roleRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.roleService.UpdateRoleAsync(id, roleRequestDto, cancellationToken);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
        {
            var result = await this.roleService.DeleteRoleAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost("DeleteSelectedList")]
        public async Task<IActionResult> DeleteSelected([FromBody] List<string> idList, CancellationToken cancellationToken = default)
        {
            var result = await this.roleService.DeleteAllByIdListAsync(idList, cancellationToken);
            return Ok(result);

        }
    }
}
