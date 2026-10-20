using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.CategoryNewsUrl;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<CategoryNewsUrlRequestDto, CategoryNewsUrlResponseDto>))]
    [CustomAuthorizationFilter<CategoryNewsUrl>]
    public class CategoryNewsUrlsController : BaseController<CategoryNewsUrlRequestDto, CategoryNewsUrl, CategoryNewsUrlResponseDto>
    {
    }
}
