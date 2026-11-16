namespace HaberApp.Core.DTOs.Others
{
    public class ClaudFlareImageResponse
    {
        public bool Success { get; set; }
        public CloudflareImageResult Result { get; set; }
    }

    public class CloudflareImageResult
    {
        public string Id { get; set; }
        public string Filename { get; set; }
        public string Uploaded { get; set; }
        public List<string> Variants { get; set; }
    }
}
