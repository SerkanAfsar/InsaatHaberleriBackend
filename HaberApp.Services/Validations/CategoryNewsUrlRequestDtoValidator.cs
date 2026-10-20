using FluentValidation;
using HaberApp.Core.DTOs.CategoryNewsUrl;
using HaberApp.Core.Repositories;

namespace HaberApp.Services.Validations
{
    public class CategoryNewsUrlRequestDtoValidator : AbstractValidator<CategoryNewsUrlRequestDto>
    {
        private readonly ICategoryNewsUrlRepository categoryNewsUrlRepository;

        public CategoryNewsUrlRequestDtoValidator(ICategoryNewsUrlRepository categoryNewsUrlRepository)
        {
            this.categoryNewsUrlRepository = categoryNewsUrlRepository;

            RuleFor(a => a.CategoryId).Cascade(CascadeMode.Stop)
                .Must(a => !string.IsNullOrEmpty(a.ToString())).WithMessage("Kategori Seçiniz")
                .GreaterThan(0).WithMessage("Kategori Seçiniz");


            RuleFor(a => a.Url).Cascade(CascadeMode.Stop)
                .Must(a => !string.IsNullOrEmpty(a)).WithMessage("Url Giriniz")
                .MustAsync(async
                (ctx, value, token) =>
                {
                    var exists = await this.categoryNewsUrlRepository.FindFirstByFilterAsyc(a => a.CategoryId == ctx.CategoryId && a.NewsSite == ctx.newsSite && a.Url == ctx.Url && a.Id != ctx.id, token);
                    return exists == null;
                }).WithMessage("Bu Url Kayıtlı");

            RuleFor(a => a.newsSite).Must(a => !string.IsNullOrEmpty(a.ToString())).WithMessage("Haber Sayfası Tipi Seçiniz");
        }
    }
}
