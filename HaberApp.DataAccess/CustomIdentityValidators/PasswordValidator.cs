using HaberApp.Core.Domains;
using Microsoft.AspNetCore.Identity;

namespace HaberApp.DataAccess.CustomIdentityValidators
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            List<IdentityError> errors = new List<IdentityError>();
            if (user.UserName!.ToLower().Contains(password!.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "Test1",
                    Description = "Şifre Kullanıcı Adını İçeremez"
                });
            }
            if (password.ToLower().StartsWith("1234"))
            {
                errors.Add(new IdentityError()
                {
                    Code = "Test2",
                    Description = "Şifre Ardışık Sayılar İle Başlayamaz"
                });
            }
            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
