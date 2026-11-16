using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Services;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<CategoryRequestDto, CategoryResponseDto>))]
    [CustomAuthorizationFilter<Category>]
    public class CategoriesController : BaseController<CategoryRequestDto, Category, CategoryResponseDto>
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }


        [HttpGet("MostReaded")]
        public async Task<IActionResult> GetMostReaded(CancellationToken cancellationToken = default)
        {
            var result = await this.categoryService.GetMostReadedWithArticlesAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("RandomCategories")]
        public async Task<IActionResult> RandomCategories(CancellationToken cancellationToken = default)
        {
            var result = await this.categoryService.GetRandomCategoryWithNews(cancellationToken);
            return Ok(result);
        }

        public async override Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await this.categoryService.GetCategoryListWithNewsCountAsync(cancellationToken);
            return Ok(result);
        }



        public override async Task<IActionResult> GetAllPaginated([FromQuery] GetAllDataTypeRequestDto requestData, CancellationToken cancellationToken = default)
        {
            var result = await this.categoryService.GetAllByQueryAsync(
                            requestData,
                            a => string.IsNullOrEmpty(requestData.GlobalFilter)
                                 || a.CategoryName.ToLower().Contains(requestData.GlobalFilter.ToLower())
                                 || a.Id.ToString() == requestData.GlobalFilter.ToString(),
                            null,
                            cancellationToken
                        );

            return Ok(result);
        }
    }
}
