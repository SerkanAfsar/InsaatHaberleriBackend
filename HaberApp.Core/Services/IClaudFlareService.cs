using Microsoft.AspNetCore.Http;

namespace HaberApp.Core.Services
{
    public interface IClaudFlareService
    {
        Task<string> UploadImage(IFormFile file);
        Task<bool> DeleteImage(string imageId);

        Task ValidateToken(string token, CancellationToken cancellationToken = default);

    }
}
