using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Utils;

namespace HaberApp.Core.Services
{
    public interface IRoleService
    {
        Task<ResponseResult<RoleResponseDto>> GetRoleByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ResponseResult<RoleResponseDto>> GetAllRolesAsync(GetAllDataTypeRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<ResponseResult<RoleResponseDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
        Task<ResponseResult<RoleResponseDto>> CreateRoleAsync(RoleRequestDto role, CancellationToken cancellationToken = default);
        Task<ResponseResult<RoleResponseDto>> UpdateRoleAsync(string id, RoleRequestDto role, CancellationToken cancellationToken = default);
        Task<ResponseResult<RoleResponseDto>> DeleteRoleAsync(string id, CancellationToken cancellationToken = default);
        Task<ResponseResult<RoleResponseDto>> DeleteAllByIdListAsync(List<string> roleIds, CancellationToken cancellationToken = default);
    }
}
