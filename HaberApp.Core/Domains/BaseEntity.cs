using System.ComponentModel.DataAnnotations;

namespace HaberApp.Core.Domains
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public string CreatedById { get; set; } = string.Empty;
        public string UpdatedById { get; set; } = string.Empty;
    }
}
