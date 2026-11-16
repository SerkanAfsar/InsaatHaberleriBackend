using FluentValidation;
using HaberApp.Core.DTOs.Others;

namespace HaberApp.Services.Validations
{
    public class AppRoleRequestDtoValidator : AbstractValidator<RoleRequestDto>
    {
        public AppRoleRequestDtoValidator()
        {

        }
    }
}
