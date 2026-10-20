using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.Services;
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
        private readonly IArticleImportService articleImportService;

        public ArticlesController(IArticleImportService articleImportService)
        {
            this.articleImportService = articleImportService;
        }

        [HttpGet("Deneme")]
        public async Task<IActionResult> Test(CancellationToken cancellationToken = default)
        {
            var result = await articleImportService.ImportNewArticlesAsync(cancellationToken);
            return Ok(result);
        }
    }
}
