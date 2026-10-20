using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace HaberApp.WebService.Middlewares;

public sealed class HangfireDashboardBasicAuthMiddleware(
    RequestDelegate next,
    string username,
    string password)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.CacheControl = "no-store";

        if (!context.Request.IsHttps)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        if (!HasValidCredentials(context.Request.Headers.Authorization.ToString(), username, password))
        {
            context.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire\", charset=\"UTF-8\"";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }

    private static bool HasValidCredentials(string header, string username, string password)
    {
        if (header.Length > 8192 ||
            !AuthenticationHeaderValue.TryParse(header, out var authorization) ||
            !string.Equals(authorization.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrEmpty(authorization.Parameter))
        {
            return false;
        }

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(authorization.Parameter));
            var separator = decoded.IndexOf(':');
            if (separator < 0 || !string.Equals(decoded[..separator], username, StringComparison.Ordinal))
                return false;

            var providedPassword = Encoding.UTF8.GetBytes(decoded[(separator + 1)..]);
            var expectedPassword = Encoding.UTF8.GetBytes(password);
            return CryptographicOperations.FixedTimeEquals(providedPassword, expectedPassword);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
