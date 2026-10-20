using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<ArticleRequestDto, ArticleResponseDto>))]
    [CustomAuthorizationFilter<Article>]
    public class ArticlesController : BaseController<ArticleRequestDto, Article, ArticleResponseDto>
    {
    }
}
