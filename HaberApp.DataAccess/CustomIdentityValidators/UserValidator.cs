using HaberApp.Core.Domains;
using Microsoft.AspNetCore.Identity;

namespace HaberApp.DataAccess.CustomIdentityValidators
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new();

            var isDigit = int.TryParse(user.UserName![0].ToString(), out int _);

            if (isDigit)
            {
                errors.Add(new IdentityError()
                {
                    Code = "Test3",
                    Description = "Kullanıcı Adının İlk Harfi Rakam İle Başlayamaz"
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
