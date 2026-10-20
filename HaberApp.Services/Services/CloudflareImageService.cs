using System.Net.Http.Headers;
using System.Text.Json;
using HaberApp.Core.Models;
using HaberApp.Core.Services;
using Microsoft.Extensions.Options;

namespace HaberApp.Services.Services
{
    public sealed class CloudflareImageService(HttpClient httpClient, IOptions<ClaudFlareSettings> options)
        : ICloudflareImageService
    {
        private const int MaxImageBytes = 10 * 1024 * 1024;

        public async Task<string> UploadFromUrlAsync(string imageUrl, CancellationToken cancellationToken = default)
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var imageUri) || imageUri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Görsel adresi geçerli bir HTTPS URL'si olmalıdır.", nameof(imageUrl));

            var settings = options.Value;
            if (string.IsNullOrWhiteSpace(settings.AccountId) || string.IsNullOrWhiteSpace(settings.AccountToken))
                throw new InvalidOperationException("ClaudFlareSettings.AccountId ve AccountToken yapılandırılmalıdır.");

            using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, imageUri);
            using var downloadResponse = await httpClient.SendAsync(
                downloadRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            downloadResponse.EnsureSuccessStatusCode();

            var mediaType = downloadResponse.Content.Headers.ContentType?.MediaType;
            if (mediaType is not null && !mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Kaynak URL bir görsel döndürmedi.");
            if (downloadResponse.Content.Headers.ContentLength > MaxImageBytes)
                throw new InvalidDataException("Görsel Cloudflare Images 10 MB sınırını aşıyor.");

            using var imageData = new MemoryStream();
            await using (var imageStream = await downloadResponse.Content.ReadAsStreamAsync(cancellationToken))
            {
                var chunk = new byte[81920];
                int read;
                while ((read = await imageStream.ReadAsync(chunk, cancellationToken)) > 0)
                {
                    if (imageData.Length + read > MaxImageBytes)
                        throw new InvalidDataException("Görsel Cloudflare Images 10 MB sınırını aşıyor.");
                    await imageData.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
                }
            }

            if (imageData.Length == 0)
                throw new InvalidDataException("Kaynak görsel boş.");
            imageData.Position = 0;

            var fileName = Path.GetFileName(imageUri.AbsolutePath);
            if (string.IsNullOrWhiteSpace(fileName)) fileName = "article-image";
            if (fileName.Length > 200) fileName = fileName[^200..];

            using var fileContent = new StreamContent(imageData);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType ?? "application/octet-stream");
            using var form = new MultipartFormDataContent();
            form.Add(fileContent, "file", fileName);

            var endpoint = $"https://api.cloudflare.com/client/v4/accounts/{Uri.EscapeDataString(settings.AccountId)}/images/v1";
            using var uploadRequest = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = form };
            uploadRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.AccountToken);
            using var uploadResponse = await httpClient.SendAsync(uploadRequest, cancellationToken);
            uploadResponse.EnsureSuccessStatusCode();

            await using var responseStream = await uploadResponse.Content.ReadAsStreamAsync(cancellationToken);
            using var json = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
            var root = json.RootElement;
            if (!root.TryGetProperty("success", out var success) || success.ValueKind != JsonValueKind.True ||
                !root.TryGetProperty("result", out var result) || result.ValueKind != JsonValueKind.Object ||
                !result.TryGetProperty("id", out var id) || id.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(id.GetString()))
                throw new InvalidDataException("Cloudflare Images geçerli bir image ID döndürmedi.");

            return id.GetString()!;
        }
    }
}
