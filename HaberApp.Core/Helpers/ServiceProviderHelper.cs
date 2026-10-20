using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HaberApp.Core.Helpers
{
    public static class ServiceProviderHelper
    {
        private static IHttpContextAccessor _httpContextAccessor = default!;

        public static void InitServiceProvider(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public static T? GetService<T>()
        {
            return _httpContextAccessor.HttpContext.RequestServices.GetService<T>();
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            return _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<T>();
        }
    }
}
