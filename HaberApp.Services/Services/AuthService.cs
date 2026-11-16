using AutoMapper;
using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.AppUser;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Exceptions;
using HaberApp.Core.Helpers;
using HaberApp.Core.Models;
using HaberApp.Core.Repositories;
using HaberApp.Core.Services;
using HaberApp.Core.UnitOfWorks;
using HaberApp.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HaberApp.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepositoryBase<UserRefreshToken> refreshTokenRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AppUser> userManager;
        private readonly TokenSettings options;
        private readonly ResponseResult<TokenResponseDto> responseResult;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IClaudFlareService claudFlareService;

        public AuthService(IRepositoryBase<UserRefreshToken> repository,
         IUnitOfWork unitOfWork,
         UserManager<AppUser> userManager,
         IOptions<TokenSettings> options = null,
         IHttpContextAccessor httpContextAccessor = null,
         IMapper mapper = null,

         RoleManager<AppRole> roleManager = null,
         IClaudFlareService claudFlareService = null)
        {
            this.refreshTokenRepository = repository;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.options = options.Value;
            this.responseResult = new();
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
            this.roleManager = roleManager;
            this.claudFlareService = claudFlareService;
        }

        public async Task<ResponseResult<TokenResponseDto>> CreateTokenAsync(LoginRequestDto loginDto, CancellationToken cancellationToken = default)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }

            await this.claudFlareService.ValidateToken(loginDto.Token, cancellationToken);

            var user = await userManager.FindByEmailAsync(loginDto.EMail);
            if (user == null)
            {
                throw new CustomException("eMail", "Kullanıcı Bulunamadı", System.Net.HttpStatusCode.BadRequest);
            }
            var passwordCheck = await userManager.CheckPasswordAsync(user, loginDto.Password);

            if (passwordCheck == false)
            {
                throw new CustomException("password", "Şifre Yanlış", System.Net.HttpStatusCode.BadRequest);
            }

            return await CreateAccessAndRefreshTokenProcess(user, cancellationToken);
        }
        public async Task<ResponseResult<TokenResponseDto>> CreateAccessTokenByRefreshTokenAysnc(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrEmpty(refreshToken))
            {
                throw new CustomException("error", "Refresh Token Not Found 1", System.Net.HttpStatusCode.Unauthorized);
            }

            var existRefreshToken = await this.refreshTokenRepository.FindFirstByFilterAsyc(x => x.Code == refreshToken && x.Expiration >= DateTime.UtcNow, cancellationToken);

            if (existRefreshToken == null)
            {
                throw new CustomException("error", "Refresh Token Not Found 2", System.Net.HttpStatusCode.Unauthorized);
            }

            var user = await this.userManager.FindByIdAsync(existRefreshToken.AppUserId);

            if (user == null)
            {

                throw new CustomException("eMail", "User Not Found", System.Net.HttpStatusCode.Unauthorized);
            }

            return await CreateAccessAndRefreshTokenProcess(user, cancellationToken);
        }
        public async Task<ResponseResult<TokenResponseDto>> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new CustomException("tokenError", "Refresh Token Not Found 1", System.Net.HttpStatusCode.NotFound);
            }
            var existedToken = await this.refreshTokenRepository.FindFirstByFilterAsyc(a => a.Code == refreshToken);
            if (existedToken == null)
            {
                throw new CustomException("tokenError", "Refresh Token Not Found 2", System.Net.HttpStatusCode.NotFound);
            }

            var result = await this.refreshTokenRepository.DeleteAsync(existedToken.Id, cancellationToken);
            await this.unitOfWork.CommitAsync(cancellationToken);
            this.responseResult.Entity = new TokenResponseDto(null, null, existedToken.Code, null);
            return this.responseResult;
        }
        public async Task<ResponseResult<AppUserResponseDto>> AuthMeAsync(CancellationToken cancellationToken = default)
        {
            var identitUser = this.httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identitUser == null)
            {
                throw new CustomException("error", "UnAuthorized", System.Net.HttpStatusCode.Unauthorized);
            }
            var appUser = await userManager.FindByIdAsync(identitUser);
            if (appUser == null)
            {
                throw new CustomException("error", "User Not Found", System.Net.HttpStatusCode.NotFound);
            }
            var responseresult = new ResponseResult<AppUserResponseDto>()
            {
                Entities = null,
                Entity = this.mapper.Map<AppUserResponseDto>(appUser),
                Errors = null,
                StatusCode = System.Net.HttpStatusCode.OK,
                Success = true
            };
            return responseresult;
        }
        public async Task<ResponseResult<TokenResponseDto>> ConfirmMailAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            if (userId == null || token == null)
                throw new CustomException("error", "User veya Token Bulunamadı", System.Net.HttpStatusCode.BadRequest);

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new CustomException("error", "Kullanıcı Bulunamadı", System.Net.HttpStatusCode.NotFound);

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                throw new CustomException("error", "Mail Adresi Onaylanamadı", System.Net.HttpStatusCode.BadRequest);
            }


            this.responseResult.Success = true;
            this.responseResult.Errors = new();
            this.responseResult.Entity = TokenResponseDto.CreateNull();
            this.responseResult.StatusCode = System.Net.HttpStatusCode.OK;
            return this.responseResult;
        }
        private string CreateRefreshToken()

        {
            var numberByte = new Byte[32];

            using var rnd = RandomNumberGenerator.Create();

            rnd.GetBytes(numberByte);

            return Convert.ToBase64String(numberByte);
        }
        private IEnumerable<Claim> GetClaims(AppUser userApp, string audience, List<string> roleList, List<string> roleClaims)
        {
            var userList = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier,userApp.Id),
            new Claim(JwtRegisteredClaimNames.Email, userApp.Email),
            new Claim(ClaimTypes.Name,userApp.UserName),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Aud, audience),
            };

            roleList.ForEach(role =>
            {
                userList.Add(new Claim(ClaimTypes.Role, role));
            });

            roleClaims.ForEach((roleClaim) =>
            {
                userList.Add(new Claim("Permission", roleClaim));
            });

            return userList;
        }
        private async Task<ResponseResult<TokenResponseDto>> CreateAccessAndRefreshTokenProcess(AppUser user, CancellationToken cancellationToken = default)
        {
            var roleList = await this.userManager.GetRolesAsync(user);

            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(this.options.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(this.options.RefreshTokenExpiration);
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.options.SecurityKey));

            var refreshToken = CreateRefreshToken();

            var userRefreshToken = await this.refreshTokenRepository.FindFirstByFilterAsyc(x => x.AppUserId == user.Id);

            if (userRefreshToken == null)
            {
                await this.refreshTokenRepository.CreateAsync(new UserRefreshToken { AppUserId = user.Id, Code = refreshToken, Expiration = refreshTokenExpiration });
            }
            else
            {
                userRefreshToken.Code = refreshToken;
                userRefreshToken.Expiration = refreshTokenExpiration;
                await this.refreshTokenRepository.UpdateAsync(userRefreshToken.Id, userRefreshToken, cancellationToken);
            }

            await this.unitOfWork.CommitAsync(cancellationToken);


            var roleClaims = new List<string>();


            for (int i = 0; i < roleList.Count(); i++)
            {
                var role = await this.roleManager.FindByNameAsync(roleList[i]);
                if (role != null)
                {
                    var claimList = await roleManager.GetClaimsAsync(role);
                    claimList.ToList().ForEach(elem =>
                    {
                        roleClaims.Add(elem.Value);
                    });
                }
            }


            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: this.options.Issuer,
                audience: this.options.Audience,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                expires: accessTokenExpiration,
                claims: GetClaims(user, this.options.Audience, roleList.ToList(), roleClaims)
                );

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            this.responseResult.Success = true;

            this.responseResult.Entity = new TokenResponseDto(token, accessTokenExpiration, refreshToken, refreshTokenExpiration);
            this.responseResult.StatusCode = System.Net.HttpStatusCode.OK;
            return this.responseResult;

        }

        //public Task<ResponseResult<TokenResponseDto>> ConfirmMailAsync(ConfirmRequestDto requestDto, CancellationToken ct = default)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ResponseResult<AppUserResponseDto>> CreateUserProcessAsync(AppUserRequestDto requestDto, CancellationToken ct = default)
        {

            var rootAdminRoleId = await RoleHelpers.FindRootAdminRoleId();
            if (string.IsNullOrEmpty(rootAdminRoleId))
            {
                throw new CustomException("error", "Root Admin Rolü Tanımlayınız");
            }

            if (requestDto.Roles.Contains(rootAdminRoleId))
            {
                throw new CustomException("error", "Root Admin Rolünde Yanlızca Bir Adet Kullanıcı Olabilir");
            }

            var user = new AppUser()
            {
                Email = requestDto.EMail,
                UserName = requestDto.EMail,
                Name = requestDto.Name,
                Surname = requestDto.Surname,
                PictureUrl = requestDto.PictureUrl,

            };

            var identityUser = await userManager.CreateAsync(user, requestDto.Password);
            if (!identityUser.Succeeded)
            {
                var error = identityUser.Errors.First();
                throw new CustomException(error.Code, error.Description);
            }
            var roleNameList = new List<string>();
            foreach (var roleId in requestDto.Roles)
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role is not null)
                {
                    roleNameList.Add(role.Name!);
                }
            }
            var roleResult = await userManager.AddToRolesAsync(user, roleNameList);

            if (!roleResult.Succeeded)
            {
                var error = roleResult.Errors.First();
                await userManager.DeleteAsync(user);
                throw new CustomException(error.Code, error.Description);
            }

            return new()
            {
                Entity = new AppUserResponseDto(user.Name, user.Surname, user.Id, user.Email, user.PictureUrl),

            };

        }
    }
}
