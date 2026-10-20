namespace HaberApp.Core.Domains
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string SubDescription { get; set; }
        public string Content { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string? ImageId { get; set; } = null!;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? SourceImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
