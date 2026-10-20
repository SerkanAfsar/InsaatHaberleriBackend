using FluentValidation;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.Repositories;

namespace HaberApp.Services.Validations
{
    public class ArticleRequestDtoValidator : AbstractValidator<ArticleRequestDto>
    {

        private readonly IArticleRepository articleRepository;
        private readonly ICategoryRepository categoryRepository;

        public ArticleRequestDtoValidator(IArticleRepository articleRepository, ICategoryRepository categoryRepository)
        {
            this.articleRepository = articleRepository;
            this.categoryRepository = categoryRepository;

            RuleFor(a => a.Title).Cascade(CascadeMode.Stop)
                .Must(a => !string.IsNullOrEmpty(a)).WithMessage("Haber Başlığı Giriniz")
                .MustAsync(async (ctx, value, token) =>
                {
                    var normalizedTitle = value.ToLowerInvariant();
                    var result = await this.articleRepository.FindFirstByFilterAsyc(a => a.CategoryId == ctx.CategoryId && a.Title.ToLower() == normalizedTitle, token);
                    return result == null;

                }).WithMessage("Bu Kategoride Bu Haber Zaten Kayıtlı");

            RuleFor(a => a.CategoryId).Cascade(CascadeMode.Stop)
                .Must(a => !string.IsNullOrEmpty(a.ToString())).WithMessage("Kategori Seçiniz")
                .GreaterThan(0).WithMessage("Kategori Seçiniz")
                .MustAsync(async (ctx, value, token) =>
                {
                    var exists = await this.categoryRepository.GetByIdAsync(value, token);
                    return exists != null;
                }).WithMessage("Kategori Kayıtlı Değil");


            RuleFor(a => a.SubTitle).Must(a => !string.IsNullOrEmpty(a)).WithMessage("Sub Title Giriniz");

            RuleFor(a => a.Content).Must(a => !string.IsNullOrEmpty(a)).WithMessage("İçerik Giriniz");

            RuleFor(a => a.SeoTitle).Must(a => !string.IsNullOrEmpty(a)).WithMessage("Seo Title Giriniz");

            RuleFor(a => a.SeoDescription).Must(a => !string.IsNullOrEmpty(a)).WithMessage("Seo Description Giriniz");

            RuleFor(a => a.ImageFile)
                .Cascade(CascadeMode.Stop)
                .Must(file => file == null ||
                    new[] { ".jpg", ".jpeg", ".png", ".webp" }
                        .Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
                .WithMessage("Sadece JPG, JPEG, PNG ve WEBP dosyaları yüklenebilir.")
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                .WithMessage("Resim dosyası maksimum 2 MB olabilir.");
            this.categoryRepository = categoryRepository;
        }

    }
}
