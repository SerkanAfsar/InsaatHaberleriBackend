using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Services;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<ArticleRequestDto, ArticleResponseDto>))]
    [CustomAuthorizationFilter<Article>]
    public class ArticlesController : BaseController<ArticleRequestDto, Article, ArticleResponseDto>
    {
        private readonly IArticleImportService articleImportService;
        private readonly IArticlesService articlesService;

        public ArticlesController(IArticleImportService articleImportService, IArticlesService articlesService)
        {
            this.articleImportService = articleImportService;
            this.articlesService = articlesService;
        }



        public override async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
        {
            //return base.Get(id, cancellationToken);
            var result = await articlesService.FindByIdAsync(id, cancellationToken);
            return Ok(result);
        }


        [HttpPost]
        public async override Task<IActionResult> Post([FromForm] ArticleRequestDto blogDto, CancellationToken cancellationToken = default)
        {
            var result = await this.articlesService.CreateAsync(blogDto);
            return Ok(result);
        }


        [HttpPut("{id}")]
        public async override Task<IActionResult> Put(int id, [FromForm] ArticleRequestDto blogRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.articlesService.UpdateAsync(id, blogRequestDto, cancellationToken);
            return Ok(result);
        }
        [HttpGet("LastNews")]
        public async Task<IActionResult> GetLastNews([FromQuery] GetAllDataTypeRequestDto requestData, [FromQuery] int? categoryId = null, CancellationToken cancellationToken = default)
        {
            var result = await this.articlesService.GetLastNewsAsync(requestData, categoryId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("Popular")]
        public async Task<IActionResult> PopularPosts([FromQuery] int? categoryId = null, CancellationToken cancellationToken = default)
        {
            var result = await this.articlesService.GetPopularPosts(categoryId, cancellationToken);
            return Ok(result);
        }


        [HttpGet("MostPopular")]
        public async Task<IActionResult> MostPopular([FromQuery] int? categoryId = null, CancellationToken cancellationToken = default)
        {
            var result = await this.articlesService.GetMostPopularPost(categoryId, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("{id}/IncreaseReadCount")]
        public async Task<IActionResult> MostPopular(int id, CancellationToken cancellationToken = default)
        {
            var result = await this.articlesService.IncreaseReadCount(id, cancellationToken);
            return Ok(result);
        }

        public override async Task<IActionResult> GetAllPaginated([FromQuery] GetAllDataTypeRequestDto requestData, CancellationToken cancellationToken = default)
        {
            var result = await this.serviceBase.GetAllByQueryAsync(
                                requestData,
                                a =>
                                    (!requestData.CategoryId.HasValue ||
                                     a.CategoryId == requestData.CategoryId.Value)
                                    &&
                                    (string.IsNullOrEmpty(requestData.GlobalFilter)
                                     || a.Title.ToLower().Contains(requestData.GlobalFilter.ToLower())
                                     || a.Category.CategoryName.ToLower().Contains(requestData.GlobalFilter.ToLower())
                                     || a.Id.ToString() == requestData.GlobalFilter),
                                b => b.Include(c => c.Category),
                                cancellationToken
                            );

            return Ok(result);
        }
        public override async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await this.articlesService.FindAllAsync(null, a => a.Include(b => b.Category), cancellationToken);
            return Ok(result);

        }
    }
}
