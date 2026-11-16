using AutoMapper;
using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.AppUser;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Exceptions;
using HaberApp.Core.Services;
using HaberApp.Core.UnitOfWorks;
using HaberApp.Core.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text.Json;

namespace HaberApp.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly ResponseResult<AppUserResponseDto> responseResult;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        //private readonly IEmailService emailSender;
        //private readonly IRepository<Annoucement> annoucementRepository;

        public UserService(UserManager<AppUser> userManager, IMapper mapper, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.responseResult = new();
            this.mapper = mapper;
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
            //this.emailSender = emailSender;
            //this.annoucementRepository = annoucementRepository;
        }

        public async Task<ResponseResult<AppUserResponseDto>> CreateMemberProcessAsync(AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            await this.unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var user = new AppUser()
                {
                    //CityId = appUserRequestDto.CityId,
                    //Education = appUserRequestDto.Education,
                    //DistrictId = appUserRequestDto.DistrictId,
                    Email = appUserRequestDto.EMail,
                    //Job = appUserRequestDto.Job,
                    Name = appUserRequestDto.Name,
                    //Gender = appUserRequestDto.Gender,
                    Surname = appUserRequestDto.Surname,
                    UserName = appUserRequestDto.EMail,
                };
                var result = await userManager.CreateAsync(user, appUserRequestDto.Password);

                if (!result.Succeeded)
                {
                    throw new CustomException("error", result.Errors.First().Description ?? "Identity Custom Error", HttpStatusCode.BadRequest);
                }

                var role = await roleManager.FindByNameAsync("NormalUser");
                if (role != null)
                {
                    await userManager.AddToRoleAsync(user, role.Name!);
                }

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);



                //var url = $"http://localhost:3000/confirm?userId={user.Id}&token={WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token))}";


                //await this.emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Lütfen hesabınızı doğrulayın: <a href='{url}'>Tıkla</a>");

                await this.unitOfWork.CommitAsync(cancellationToken);
                this.responseResult.Entity = this.mapper.Map<AppUserResponseDto>(user);
                return this.responseResult;
            }
            catch (Exception ex)
            {
                await this.unitOfWork.RollbackAsync(cancellationToken);
                throw new CustomException("error", ex.Message, HttpStatusCode.BadRequest);
            }

        }

        public async Task<ResponseResult<AppUserResponseDto>> CreateUserProcessAsync(AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default)
        {
            var user = new AppUser()
            {
                //CityId = appUserRequestDto.CityId,
                //Education = appUserRequestDto.Education,
                //DistrictId = appUserRequestDto.DistrictId,
                Email = appUserRequestDto.EMail,
                //Job = appUserRequestDto.Job,
                Name = appUserRequestDto.Name,
                //Gender = appUserRequestDto.Gender,
                Surname = appUserRequestDto.Surname,
                UserName = appUserRequestDto.EMail,
            };
            var result = await userManager.CreateAsync(user, appUserRequestDto.Password);

            if (!result.Succeeded)
            {
                throw new CustomException("error", result.Errors.First().Description ?? "Identity Custom Error", System.Net.HttpStatusCode.BadRequest);
            }

            for (var i = 0; i < appUserRequestDto.Roles.Count; i++)
            {
                var roleName = appUserRequestDto.Roles[i];
                var existRole = await roleManager.FindByIdAsync(roleName);
                if (existRole != null)
                {
                    var roleResult = await userManager.AddToRoleAsync(user, existRole.Name!);
                    if (!roleResult.Succeeded)
                    {
                        await userManager.DeleteAsync(user);
                        throw new CustomException("error", roleResult.Errors.First().Description ?? "Role Err", HttpStatusCode.BadRequest);

                    }
                }
            }
            this.responseResult.Entity = this.mapper.Map<AppUserResponseDto>(user);
            return this.responseResult;

        }

        public async Task<ResponseResult<AppUserResponseDto>> DeleteUserProcessAsync(string userId, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new CustomException("error", "User Not Found", System.Net.HttpStatusCode.NotFound);
            }

            var isInRole = await this.userManager.IsInRoleAsync(user, RoleConstants.RootAdmin);
            if (isInRole)
            {
                throw new CustomException("error", "Root Admin Rolündeki Kullanıcı Silinemez!", System.Net.HttpStatusCode.BadRequest);
            }
            var result = await this.userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(item =>
                {
                    this.responseResult.Errors.Add(item.Code, item.Description);
                });
                this.responseResult.StatusCode = System.Net.HttpStatusCode.BadRequest;
                this.responseResult.Success = false;
                return this.responseResult;
            }
            this.responseResult.Entity = this.mapper.Map<AppUserResponseDto>(user);
            return this.responseResult;
        }

        public async Task<ResponseResult<AppUserResponseDto>> FindUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {

            var user = await this.userManager.Users
                .FirstOrDefaultAsync(a => a.Id == userId, cancellationToken);
            if (user == null)
            {
                throw new CustomException("error", "User Not Found", System.Net.HttpStatusCode.NotFound);
            }

            var roleIdList = new List<string>();
            var rolesResult = await this.userManager.GetRolesAsync(user);

            foreach (var item in rolesResult)
            {
                var role = await roleManager.FindByNameAsync(item);
                if (role is not null)
                {
                    roleIdList.Add(role.Id);
                }
            }


            this.responseResult.Entity = new AppUserResponseDto(user.Name, user.Surname, user.Id, user.Email, null, Roles: roleIdList);
            return this.responseResult;

        }

        public async Task<ResponseResult<AppUserResponseDto>> GetAllUsersByFilterAsync(GetAllDataTypeRequestDto requestDto, CancellationToken cancellationToken = default)
        {

            var result = this.userManager.Users
                .AsNoTracking()
                //.Include(a => a.Annoucements)
                .AsQueryable();
            if (requestDto.GlobalFilter != null)
            {
                result = result.Where(a => a.Name.ToLower().Contains(requestDto.GlobalFilter.ToLower()) || a.Surname.ToLower().Contains(requestDto.GlobalFilter.ToLower()) || a.Email.ToLower().Contains(requestDto.GlobalFilter.ToLower()));
            }
            if (!string.IsNullOrEmpty(requestDto.Sorting))
            {
                var sorting = JsonSerializer.Deserialize<List<SortingDto>>(requestDto.Sorting);

                if (sorting?.Count > 0)
                {
                    var sort = sorting.First();

                    var deneme = sort.id == "appUserId" ? "id" : sort.id;

                    result = result.OrderBy($"{deneme} {(sort.desc ? "desc" : "")}");
                }
            }
            var lastResult = await result.Skip(requestDto.Offset).Take(requestDto.Count).ToListAsync(cancellationToken);
            this.responseResult.Entities = this.mapper.Map<List<AppUserResponseDto>>(lastResult);
            this.responseResult.RowCount = await result.CountAsync(cancellationToken);
            return this.responseResult;
        }

        public async Task<ResponseResult<AppUserResponseDto>> UpdateUserPasswordProcessAsync(UpdatePasswordRequestDto passwordRequestDto, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var user = await this.userManager.FindByIdAsync(passwordRequestDto.UserId);
            if (user == null)
            {
                throw new CustomException("error", "Kullanıcı Bulunamadı", HttpStatusCode.NotFound);
            }

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var passwordResult = await this.userManager.ChangePasswordAsync(user, passwordRequestDto.OldPassword, passwordRequestDto.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    var err = passwordResult.Errors.FirstOrDefault();
                    if (err?.Code == "oldPassword" || err?.Code == "newPassword")
                    {
                        throw new CustomException(err.Code, err.Description, HttpStatusCode.BadRequest);
                    }
                    throw new CustomException("error", err?.Description ?? "Password Update Error", HttpStatusCode.BadRequest);
                }

                //await annoucementRepository.CreateAsync(new Annoucement
                //{
                //    AppUserId = user.Id,
                //    Title = "Şifre değiştirildi",
                //    Message = "Hesabınızın şifresi başarıyla değiştirildi."
                //}, cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }

            this.responseResult.StatusCode = HttpStatusCode.OK;
            this.responseResult.Entity = this.mapper.Map<AppUserResponseDto>(user);
            this.responseResult.Success = true;
            return this.responseResult;
        }

        public async Task<ResponseResult<AppUserResponseDto>> UpdateUserProcessAsync(string userId, AppUserRequestDto appUserRequestDto, CancellationToken cancellationToken = default)
        {
            await this.unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var user = await this.userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new CustomException("email", "Kullanıcı Bulunamadı", System.Net.HttpStatusCode.NotFound);
                }

                //user.CityId = appUserRequestDto.CityId;
                //user.DistrictId = appUserRequestDto.DistrictId;
                //user.Job = appUserRequestDto.Job;
                user.Name = appUserRequestDto.Name!;
                //user.Gender = appUserRequestDto.Gender;
                user.Surname = appUserRequestDto.Surname!;
                //user.Education = appUserRequestDto.Education;
                user.UserName = appUserRequestDto.EMail;


                if (user.Email != appUserRequestDto.EMail)
                {
                    var token = await userManager.GenerateChangeEmailTokenAsync(user, appUserRequestDto.EMail);
                    var changeEmailResult = await userManager.ChangeEmailAsync(user, appUserRequestDto.EMail, token);
                    if (!changeEmailResult.Succeeded)
                    {
                        throw new CustomException("error", changeEmailResult.Errors.First().Description ?? "Change EMail eRRor", System.Net.HttpStatusCode.BadRequest);
                    }
                }

                var passwordChanged = false;
                if (!string.IsNullOrEmpty(appUserRequestDto.Password))
                {

                    await userManager.RemovePasswordAsync(user);
                    var addPassword = await userManager.AddPasswordAsync(user, appUserRequestDto.Password);
                    if (!addPassword.Succeeded)
                    {
                        throw new CustomException("error", addPassword.Errors.First().Description ?? "Password Add eRRor", System.Net.HttpStatusCode.BadRequest);
                    }

                    passwordChanged = true;
                }


                var roles = await this.userManager.GetRolesAsync(user);
                var roleResult = await this.userManager.RemoveFromRolesAsync(user, roles);

                if (!roleResult.Succeeded)
                {
                    throw new CustomException("error", roleResult.Errors.First().Description ?? "Remove Role Result Error ", System.Net.HttpStatusCode.BadRequest);
                }

                for (var i = 0; i < appUserRequestDto.Roles.Count; i++)
                {
                    var roleName = appUserRequestDto.Roles[i];
                    var existRole = await roleManager.FindByIdAsync(roleName);
                    if (existRole != null)
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(user, existRole.Name!);
                        if (!roleResult.Succeeded)
                        {
                            await userManager.DeleteAsync(user);
                            throw new CustomException("error", roleResult.Errors.First().Description ?? "Role Err", HttpStatusCode.BadRequest);

                        }
                    }
                }


                var updateResult = await this.userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    updateResult.Errors.ToList().ForEach(err =>
                    {
                        this.responseResult.Errors.Add(err.Code, err.Description);
                    });
                    this.responseResult.StatusCode = HttpStatusCode.BadRequest;
                    this.responseResult.Success = false;
                    return this.responseResult;
                }

                //if (passwordChanged)
                //{
                //    await annoucementRepository.CreateAsync(new Annoucement
                //    {
                //        AppUserId = user.Id,
                //        Title = "Şifre değiştirildi",
                //        Message = "Hesabınızın şifresi başarıyla değiştirildi."
                //    }, cancellationToken);
                //}


                await this.unitOfWork.CommitAsync(cancellationToken);
                this.responseResult.Entity = this.mapper.Map<AppUserResponseDto>(user);
                return this.responseResult;
            }
            catch (Exception ex)
            {

                await this.unitOfWork.RollbackAsync(cancellationToken);
                throw ex;
            }
        }
    }
}
