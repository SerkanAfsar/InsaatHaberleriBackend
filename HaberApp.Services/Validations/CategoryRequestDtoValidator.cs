using FluentValidation;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.Repositories;

namespace HaberApp.Services.Validations
{
    public class CategoryRequestDtoValidator : AbstractValidator<CategoryRequestDto>
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryRequestDtoValidator(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;

            RuleFor(a => a.CategoryName)
                .Cascade(CascadeMode.Stop)
                .Must(a => !string.IsNullOrEmpty(a)).WithMessage("Kategori Adını Giriniz")
                .MustAsync(async (ctx, categoryName, token) =>
                {
                    var normalizedName = categoryName.ToLowerInvariant();
                    var exists = await this.categoryRepository.FindFirstByFilterAsyc(a => a.CategoryName.ToLower() == normalizedName && a.Id != ctx.id, token);
                    return exists == null;
                }).WithMessage("Bu Kategori Adında Kayıtlı Kategori Bulunmaktadır");

            RuleFor(a => a.SeoTitle).Must(a => !string.IsNullOrEmpty(a)).WithMessage("Seo Title Giriniz");

            RuleFor(a => a.SeoDescription).Must(a => !string.IsNullOrEmpty(a)).WithMessage("Seo Description Giriniz");

        }
    }

}
