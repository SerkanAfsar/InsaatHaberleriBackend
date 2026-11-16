using FluentValidation;
using HaberApp.Core.DTOs.Others;

namespace HaberApp.Services.Validations
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(a => a.EMail)
                .Cascade(CascadeMode.Stop)
                .Must(a => !string.IsNullOrEmpty(a)).WithMessage("EPosta Giriniz")
                .EmailAddress().WithMessage("E-Posta Adresi Giriniz");

            RuleFor(a => a.Password).Must(a => !string.IsNullOrEmpty(a)).WithMessage("Şifre Giriniz");
        }
    }
}
