using FluentValidation;
using HaberApp.Core.DTOs.AppUser;

namespace HaberApp.Services.Validations
{
    public sealed class AppUserRequestDtoValidator : AbstractValidator<AppUserRequestDto>
    {
        public AppUserRequestDtoValidator()
        {

        }
    }
}
