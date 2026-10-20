using HaberApp.Core.Enums;

namespace HaberApp.Core.Domains
{
    public class CategoryNewsUrl : BaseEntity
    {
        public string Url { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public NewsSites NewsSite { get; set; }
    }
}
