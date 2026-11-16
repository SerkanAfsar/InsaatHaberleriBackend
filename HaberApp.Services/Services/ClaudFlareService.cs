using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Exceptions;
using HaberApp.Core.Models;
using HaberApp.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HaberApp.Services.Services
{
    public class ClaudFlareService : IClaudFlareService
    {
        private readonly HttpClient _httpClient;
        private readonly ClaudFlareSettings claudFlareSettings;

        public ClaudFlareService(HttpClient httpClient, IOptions<ClaudFlareSettings> options)
        {
            this._httpClient = httpClient;
            this.claudFlareSettings = options.Value;

            _httpClient.DefaultRequestHeaders.Authorization =
           new AuthenticationHeaderValue("Bearer", this.claudFlareSettings.AccountToken);
            _httpClient.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");

        }

        public async Task<bool> DeleteImage(string imageId)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrEmpty(imageId) || string.IsNullOrEmpty(this.claudFlareSettings.AccountId) || string.IsNullOrEmpty(this.claudFlareSettings.AccountToken))
            {
                return false;
            }
            var endpoint = $"accounts/{this.claudFlareSettings.AccountId}/images/v1/{imageId}";
            var response = await this._httpClient.DeleteAsync(endpoint);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                // Read and print the error response content
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new CustomException("error", "ClaudFlare Image Delete Error", HttpStatusCode.BadRequest);

            }


        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new CustomException("error", "No File Uploaded", HttpStatusCode.BadRequest);
            }
            if (file.Length > 10 * 1024 * 1024) // 10MB limit
            {

                throw new CustomException("error", "File size exceeds 10MB limit", HttpStatusCode.BadRequest);
            }
            // Read file content
            using var stream = file.OpenReadStream();
            byte[] fileBytes = new byte[file.Length];
            await stream.ReadAsync(fileBytes);

            var boundary = Guid.NewGuid().ToString();
            var content = new MultipartFormDataContent(boundary);


            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
            content.Add(fileContent, "file", file.FileName);


            var response = await _httpClient.PostAsync(
                $"accounts/{this.claudFlareSettings.AccountId}/images/v1",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new CustomException("error", "ClaudFlare Image Err", HttpStatusCode.BadRequest);
            }



            var result = await response.Content.ReadFromJsonAsync<ClaudFlareImageResponse>();
            return result.Result.Id;

        }

        public async Task ValidateToken(string token, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, string>
            {
                ["secret"] = claudFlareSettings.TurnstileSecretKey,
                ["response"] = token
            };

            try
            {
                using var content = new FormUrlEncodedContent(data);

                using var response = await _httpClient.PostAsync(
                    "https://challenges.cloudflare.com/turnstile/v0/siteverify",
                    content,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                    throw new CustomException("error", "ClaudFlare err");

                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                var result = JsonSerializer.Deserialize<TurnstileResult>(json);
                if (result == null || result.Success != true)
                {
                    throw new CustomException("error", "ClaudFlareErr");
                }

            }
            catch (Exception ex)
            {
                throw new CustomException("error", ex.Message);
            }
        }

    }
    public class TurnstileResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("hostname")]
        public string? Hostname { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }
    }
}
