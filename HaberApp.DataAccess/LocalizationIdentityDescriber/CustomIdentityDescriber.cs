using Microsoft.AspNetCore.Identity;

namespace HaberApp.DataAccess.LocalizationIdentityDescriber
{
    public class CustomIdentityDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordMismatch()
        {
            return new IdentityError()
            {
                Code = "oldPassword",
                Description = "Eski Şifrenizi Yanlış!"
            };
        }
        public override IdentityError PasswordTooShort(int length)
        {
            //return base.PasswordTooShort(length);
            return new IdentityError()
            {
                Code = "newPassword",
                Description = $"Şifre en az {length} karakter içermeli"
            };
        }
    }
}
