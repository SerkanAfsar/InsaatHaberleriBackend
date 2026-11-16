using FluentValidation;
using HaberApp.Core.DTOs.AppUser;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.DTOs.CategoryNewsUrl;
using HaberApp.Core.DTOs.Message;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Models;
using HaberApp.Core.Services;
using HaberApp.Services.Mappers;
using HaberApp.Services.Services;
using HaberApp.Services.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HaberApp.Services.ServiceConfigurations
{
    public static class ServiceExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection RegisterAutoMapperService()
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));

                services.AddAutoMapper(
                    cfg => { },
                    typeof(ProfileMapper).Assembly
                );

                return services;
            }

            public IServiceCollection RegisterServices()
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));
                services.AddScoped(typeof(IServiceBase<,,>), typeof(ServiceBase<,,>));
                services.AddScoped<ICategoryService, CategoryService>();
                services.AddScoped<ICategoryNewsUrlService, CategoryNewsUrlService>();
                services.AddScoped<IArticlesService, ArticleService>();
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IArticleImportService, ArticleImportService>();
                services.AddScoped<IRoleService, RolesService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IMessageService, MessageService>();
                services.AddTransient<IClaudFlareService, ClaudFlareService>();

                services.AddHttpClient<InsaatDunyasiService>(ConfigureCrawlerClient);
                services.AddHttpClient<YapiComTrService>(ConfigureCrawlerClient);
                services.AddHttpClient<YapiGundemService>(ConfigureCrawlerClient);
                services.AddHttpClient<ICloudflareImageService, CloudflareImageService>(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(90);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; HaberAppCrawler/1.0)");
                });

                return services;
            }

            private static void ConfigureCrawlerClient(HttpClient client)
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; HaberAppCrawler/1.0)");
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("tr-TR,tr;q=0.9");
            }
            public IServiceCollection RegisterCloudFlareSettings(IConfiguration configuration)
            {
                if (services == null)
                {
                    throw new ArgumentNullException(nameof(services));
                }
                services.Configure<ClaudFlareSettings>(configuration.GetSection("ClaudFlareSettings"));
                return services;
            }

            public IServiceCollection RegisterValidators()
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));

                services.AddScoped<IValidator<ArticleRequestDto>, ArticleRequestDtoValidator>();
                services.AddScoped<IValidator<CategoryRequestDto>, CategoryRequestDtoValidator>();
                services.AddScoped<IValidator<CategoryNewsUrlRequestDto>, CategoryNewsUrlRequestDtoValidator>();
                services.AddScoped<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
                services.AddScoped<IValidator<RoleRequestDto>, AppRoleRequestDtoValidator>();
                services.AddScoped<IValidator<AppUserRequestDto>, AppUserRequestDtoValidator>();
                services.AddScoped<IValidator<MessageRequestDto>, MessageRequestDtoValidator>();

                return services;
            }
        }

    }
}
