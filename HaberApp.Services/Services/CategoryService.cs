using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.Services;
using HaberApp.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace HaberApp.Services.Services
{
    public class CategoryService : ServiceBase<CategoryRequestDto, Category, CategoryResponseDto>, ICategoryService
    {
        public async Task<ResponseResult<CategoryResponseDto>> GetCategoryListWithNewsCountAsync(CancellationToken cancellationToken = default)
        {
            var result = await this.appDbContext.Categories.AsNoTracking().Select(a => new
            {
                Id = a.Id!,
                CategoryName = a.CategoryName!,
                Queue = a.Queue!,
                NewsCount = a.Articles.Count(),

            }).OrderBy(a => a.Queue).ToListAsync(cancellationToken);

            var lastResult = result.Select(a => new CategoryResponseDto(Id: a.Id, CategoryName: a.CategoryName, string.Empty, string.Empty, a.Queue, null, a.NewsCount)).ToList();

            return new ResponseResult<CategoryResponseDto>()
            {
                Entities = lastResult
            };
        }

        public async Task<ResponseResult<CategoryResponseDto>> GetMostReadedWithArticlesAsync(CancellationToken cancellationToken)
        {
            var result = await appDbContext.Categories.AsNoTracking().Select(c => new
            {
                Category = c,
                TotalReadCount = c.Articles.Sum(a => a.ReadedCount),
                Articles = c.Articles.OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.ReadedCount).Take(6)
            .OrderByDescending(a => a.ReadedCount)
            .ToList()
            }).OrderByDescending(x => x.TotalReadCount)
            .Take(3)
            .Select(a => new CategoryResponseDto(a.Category.Id, a.Category.CategoryName, a.Category.SeoTitle, a.Category.SeoDescription, a.Category.Queue, this.mapper.Map<List<ArticleResponseDto>>(a.Articles)))
            .ToListAsync(cancellationToken);


            return new ResponseResult<CategoryResponseDto>()
            {
                Entities = result
            };

        }

        public async Task<ResponseResult<CategoryResponseDto>> GetRandomCategoryWithNews(CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();

            var result = await appDbContext.Categories.AsNoTracking().OrderBy(a => new Guid()).Select(a => new
            {
                Id = a.Id,
                CategoryName = a.CategoryName!,
                Articles = a.Articles.OrderByDescending(a => a.CreatedAt).Take(5).ToList()

            }).Take(2).ToListAsync(cancellationToken);

            var lastResult = result.Select(a => new CategoryResponseDto(a.Id, a.CategoryName, string.Empty, string.Empty, 1, this.mapper.Map<List<ArticleResponseDto>>(a.Articles), null)).ToList();

            return new ResponseResult<CategoryResponseDto>()
            {
                Entities = lastResult
            };
        }
    }
}
