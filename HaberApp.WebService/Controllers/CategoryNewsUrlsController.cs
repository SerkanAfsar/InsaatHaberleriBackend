using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.CategoryNewsUrl;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Services;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<CategoryNewsUrlRequestDto, CategoryNewsUrlResponseDto>))]
    [CustomAuthorizationFilter<CategoryNewsUrl>]
    public class CategoryNewsUrlsController : BaseController<CategoryNewsUrlRequestDto, CategoryNewsUrl, CategoryNewsUrlResponseDto>
    {
        private readonly ICategoryNewsUrlService categoryNewsUrlService;

        public CategoryNewsUrlsController(ICategoryNewsUrlService categoryNewsUrlService)
        {
            this.categoryNewsUrlService = categoryNewsUrlService;
        }

        [HttpGet("Paginated")]
        public override async Task<IActionResult> GetAllPaginated([FromQuery] GetAllDataTypeRequestDto requestData, CancellationToken cancellationToken = default)
        {
            var paginatedResult = await this.serviceBase.GetAllByQueryAsync(requestData,
             a => string.IsNullOrEmpty(requestData.GlobalFilter)
                                 || a.Category.CategoryName.ToLower().Contains(requestData.GlobalFilter.ToLower())
                                 || a.Id.ToString() == requestData.GlobalFilter.ToString()
                                 || a.Url.ToLower().Contains(requestData.GlobalFilter.ToLower())
                                 || a.NewsSite.ToString().ToLower().Contains(requestData.GlobalFilter.ToLower())
                                 ,
                            a => a.Include(b => b.Category),
          cancellationToken
      );

            return Ok(paginatedResult);
        }
    }
}
