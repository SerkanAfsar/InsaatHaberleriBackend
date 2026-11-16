using HaberApp.Core.Domains;
using HaberApp.Core.Models;
using HaberApp.Core.Repositories;
using HaberApp.Core.UnitOfWorks;
using HaberApp.DataAccess.Contexts;
using HaberApp.DataAccess.CustomIdentityValidators;
using HaberApp.DataAccess.LocalizationIdentityDescriber;
using HaberApp.DataAccess.Repositories;
using HaberApp.DataAccess.UnitOfWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HaberApp.DataAccess.Configurations
{
    public static class DataAccessConfigurations
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection RegisterRepositories()
            {
                if (services is null) throw new ArgumentNullException("Service Must Be Setted");

                services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                services.AddScoped<IArticleRepository, ArticleRepository>();
                services.AddScoped<ICategoryNewsUrlRepository, CategoryNewsUrlRepository>();
                services.AddScoped<IMessageRepository, MessageRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                return services;
            }
            public IServiceCollection RegisterDbContext(IConfiguration configuration)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));
                services.AddDbContext<AppDbContext>(opt =>
                {
                    opt.UseSqlServer(configuration.GetConnectionString("SqlCon"), opt =>
                    {
                        opt.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    });
                });
                return services;
            }

            public IServiceCollection RegisterIdentity()
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));


                services.AddIdentityCore<AppUser>(opt =>
                {
                    opt.User.RequireUniqueEmail = true;
                    opt.SignIn.RequireConfirmedEmail = true;
                    //opt.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890@_-";




                    opt.Password.RequiredLength = 6;
                    opt.Password.RequireNonAlphanumeric = true;
                    opt.Password.RequireDigit = true;
                    opt.Password.RequireLowercase = true;
                    opt.Password.RequireUppercase = true;



                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                    opt.Lockout.MaxFailedAccessAttempts = 3;

                }).AddRoles<AppRole>()
                .AddPasswordValidator<PasswordValidator>()
                .AddUserValidator<UserValidator>()
                .AddErrorDescriber<CustomIdentityDescriber>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

                return services;
            }
            public IServiceCollection RegisterAuth(IConfiguration Configuration)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));

                services.Configure<TokenSettings>(Configuration.GetSection("TokenSettings"));

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    var tokenOptions = Configuration.GetSection("TokenSettings").Get<TokenSettings>();
                    opts.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenOptions.SecurityKey)),
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                    };
                });

                return services;
            }
        }

    }
}
