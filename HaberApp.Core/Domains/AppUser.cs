using Microsoft.AspNetCore.Identity;

namespace HaberApp.Core.Domains
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? PictureUrl { get; set; }

    }
}
