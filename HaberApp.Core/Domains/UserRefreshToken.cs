namespace HaberApp.Core.Domains
{
    public class UserRefreshToken : BaseEntity
    {
        public string AppUserId { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}
