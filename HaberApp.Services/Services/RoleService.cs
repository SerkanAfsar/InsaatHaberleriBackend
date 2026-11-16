using AutoMapper;
using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Exceptions;
using HaberApp.Core.Extensions;
using HaberApp.Core.Services;
using HaberApp.Core.Utils;
using HaberApp.DataAccess.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace HaberApp.Services.Services
{
    public class RolesService : IRoleService
    {
        private readonly RoleManager<AppRole> roleManager;
        private readonly ResponseResult<RoleResponseDto> responseResult;
        private readonly AppDbContext appDbContext;
        private readonly IMapper mapper;

        public RolesService(RoleManager<AppRole> roleManager, AppDbContext appDbContext = null, IMapper mapper = null)
        {
            this.roleManager = roleManager;
            this.responseResult = new ResponseResult<RoleResponseDto>();
            this.appDbContext = appDbContext;
            this.mapper = mapper;
        }

        public async Task<ResponseResult<RoleResponseDto>> CreateRoleAsync(RoleRequestDto role, CancellationToken cancellationToken = default)
        {

            var existedRole = await this.roleManager.FindByNameAsync(role.RoleName);
            if (existedRole != null)
            {
                throw new CustomException("error", "Role Exists", HttpStatusCode.BadRequest);
            }
            var newRole = new AppRole()
            {
                Name = role.RoleName
            };
            var newRoleResult = await this.roleManager.CreateAsync(newRole);
            if (!newRoleResult.Succeeded)
            {
                throw new CustomException("error", newRoleResult.Errors.First().Description, HttpStatusCode.BadRequest);
            }


            foreach (var claim in role.Claims)
            {
                await roleManager.AddClaimAsync(newRole, new Claim("Permission", claim));
            }

            //this.responseResult.Entity = new RoleResponseDto()
            //{
            //    Id = null,
            //    RoleId = newRole.Id,
            //    RoleName = newRole.Name
            //};
            this.responseResult.Entity = new RoleResponseDto(RoleId: newRole.Id, RoleName: newRole.Name);
            return this.responseResult;


        }

        public async Task<ResponseResult<RoleResponseDto>> DeleteAllByIdListAsync(List<string> roleIds, CancellationToken cancellationToken = default)
        {


            var result = new List<RoleResponseDto>();

            for (int i = 0; i < roleIds.Count; i++)
            {
                var appRole = await this.roleManager.FindByIdAsync(roleIds[i]);
                if (appRole != null)
                {
                    if (!RoleConstants.IsInRole(appRole.Name!))
                    {
                        var deleteResult = await this.roleManager.DeleteAsync(appRole);
                        if (deleteResult.Succeeded)
                        {
                            result.Add(this.mapper.Map<RoleResponseDto>(appRole));
                        }
                    }

                }
            }
            this.responseResult.Entities = result;
            return this.responseResult;

        }

        public async Task<ResponseResult<RoleResponseDto>> DeleteRoleAsync(string id, CancellationToken cancellationToken = default)
        {

            var result = await this.roleManager.FindByIdAsync(id);
            if (result == null)
            {
                throw new CustomException("error", "Role Not Found", HttpStatusCode.NotFound);
            }
            if (RoleConstants.IsInRole(result.Name!))
            {
                throw new CustomException("error", "Default Roller Silinemez", HttpStatusCode.BadRequest);
            }
            var dataResult = await this.roleManager.DeleteAsync(result);

            if (dataResult.Succeeded == false)
            {
                dataResult.Errors.ToList().ForEach(err =>
                {
                    this.responseResult.Errors.Add(err.Code, err.Description);
                });
                this.responseResult.Success = false;
                this.responseResult.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return this.responseResult;

            }
            this.responseResult.Entity = this.mapper.Map<RoleResponseDto>(result);
            return this.responseResult;


        }

        public async Task<ResponseResult<RoleResponseDto>> GetAllRolesAsync(GetAllDataTypeRequestDto filter, CancellationToken cancellationToken = default)
        {
            var result = this.appDbContext.Roles.AsNoTracking().AsQueryable();
            if (filter.GlobalFilter != null)
            {
                result = result.Where(a => a.Name.ToLower().Contains(filter.GlobalFilter.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.Sorting))
            {
                var sorting = JsonSerializer.Deserialize<List<SortingDto>>(filter.Sorting);

                if (sorting?.Count > 0)
                {
                    var sort = sorting.First();

                    result = result.OrderBy($"{sort.id} {(sort.desc ? "desc" : "")}");
                }
            }
            var lastResult = await result.Skip(filter.Offset).Take(filter.Count).ToListAsync(cancellationToken);
            this.responseResult.Entities = this.mapper.Map<List<RoleResponseDto>>(lastResult);
            this.responseResult.RowCount = await result.CountAsync(cancellationToken);
            return this.responseResult;

        }

        public async Task<ResponseResult<RoleResponseDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var result = this.appDbContext.Roles.AsNoTracking().AsQueryable();

            var lastResult = await result.ToListAsync(cancellationToken);
            this.responseResult.Entities = this.mapper.Map<List<RoleResponseDto>>(lastResult);
            this.responseResult.RowCount = await result.CountAsync(cancellationToken);
            return this.responseResult;
        }

        public async Task<ResponseResult<RoleResponseDto>> GetRoleByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var role = await this.roleManager.FindByIdAsync(id);
            if (role == null)
            {
                throw new CustomException("error", "Role Not Found", System.Net.HttpStatusCode.NotFound);
            }
            var claimsList = await this.roleManager.GetClaimsAsync(role);

            //var responseDto = new RoleResponseDto()
            //{
            //    RoleName = role.Name,
            //    RoleId = role.Id,
            //    Claims = claimsList.Select(a => a.Value).ToList()
            //};
            this.responseResult.Entity = new RoleResponseDto(RoleName: role.Name!, RoleId: role.Id, Claims: claimsList.Select(a => a.Value).ToList());


            return this.responseResult;
        }

        public async Task<ResponseResult<RoleResponseDto>> UpdateRoleAsync(string id, RoleRequestDto role, CancellationToken cancellationToken = default)
        {
            if (role.RoleName == "RootAdmin")
            {
                throw new CustomException("error", "Root Admin Rolü Güncellenemez", System.Net.HttpStatusCode.BadRequest);
            }

            var roleItem = await this.roleManager.FindByIdAsync(id);
            if (roleItem == null)
            {
                throw new CustomException("error", "Role Not Found", System.Net.HttpStatusCode.NotFound);
            }

            var claimsList = await this.roleManager.GetClaimsAsync(roleItem);

            for (var i = 0; i < claimsList.Count; i++)
            {
                await this.roleManager.RemoveClaimAsync(roleItem, claimsList[i]);
            }

            roleItem.Name = role.RoleName;

            var updateResult = await this.roleManager.UpdateAsync(roleItem);
            if (updateResult.Succeeded)
            {
                for (int i = 0; i < role.Claims.Count; i++)
                {
                    await this.roleManager.AddClaimAsync(roleItem, new Claim("Permission", role.Claims[i]));
                }
                this.responseResult.Entity = this.mapper.Map<RoleResponseDto>(roleItem);
                return this.responseResult;
            }

            updateResult.Errors.ToList().ForEach(err =>
            {
                this.responseResult.Errors.Add(err.Code.ToCamelCase(), err.Description);
            });
            this.responseResult.Success = false;
            this.responseResult.StatusCode = System.Net.HttpStatusCode.BadRequest;
            return this.responseResult;


        }
    }
}
