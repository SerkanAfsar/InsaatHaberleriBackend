using HaberApp.Core.Abstract;

namespace HaberApp.Core.Domains
{
    public class Category : BaseEntity, IQueuable
    {
        public string CategoryName { get; set; } = null!;
        public string SeoTitle { get; set; } = null!;
        public string SeoDescription { get; set; } = null!;
        public int Queue { get; set; }
        public List<Article> Articles { get; set; } = new();
        public List<CategoryNewsUrl> CategoryNewsUrls { get; set; } = new();
    }
}
