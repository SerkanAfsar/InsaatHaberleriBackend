using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace HaberApp.Core.Helpers
{
    public static class ContextHelpers
    {
        private static IHttpContextAccessor? httpContextAccessor;

        public static void SetContext(IServiceProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);

            httpContextAccessor =
                provider.GetRequiredService<IHttpContextAccessor>();
        }

        public static HttpContext? CurrentContext =>
            (httpContextAccessor
                ?? throw new InvalidOperationException(
                    "Önce ContextHelpers.SetContext çağrılmalıdır."))
            .HttpContext;

        public static ClaimsPrincipal? CurrentUser =>
            CurrentContext?.User;

        public static string? CurrentUserId =>
            CurrentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public static bool IsAuthenticated =>
            CurrentUser?.Identity?.IsAuthenticated == true;
    }
}
