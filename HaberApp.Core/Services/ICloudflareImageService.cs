namespace HaberApp.Core.Services
{
    public interface ICloudflareImageService
    {
        Task<string> UploadFromUrlAsync(string imageUrl, CancellationToken cancellationToken = default);
    }
}
