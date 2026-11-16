using FluentValidation;
using HaberApp.Core.DTOs.Message;

namespace HaberApp.Services.Validations
{
    public class MessageRequestDtoValidator : AbstractValidator<MessageRequestDto>
    {
        public MessageRequestDtoValidator()
        {
            RuleFor(a => a.NameSurname).Must(a => !string.IsNullOrEmpty(a)).WithMessage("İsim Soyisim Giriniz");

            RuleFor(a => a.EMail)
                .Cascade(CascadeMode.Stop)
                .Must(a => !string.IsNullOrEmpty(a)).WithMessage("E-Posta Giriniz")
                .EmailAddress().WithMessage("E-Posta Formatı Yanlış");

            RuleFor(a => a.Subject).Must(a => !string.IsNullOrEmpty(a)).WithMessage("Konu Giriniz");

            RuleFor(a => a.MessageText).Must(a => !string.IsNullOrEmpty(a)).WithMessage("Mesajınızı Giriniz");
        }
    }
}
