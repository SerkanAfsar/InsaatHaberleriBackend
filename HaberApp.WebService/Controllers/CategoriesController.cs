using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Category;
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
    }
}
