using FluentValidation;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.DTOs.CategoryNewsUrl;
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

                return services;
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

                return services;
            }
        }

    }
}
