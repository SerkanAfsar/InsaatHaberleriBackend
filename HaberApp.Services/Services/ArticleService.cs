using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Exceptions;
using HaberApp.Core.Repositories;
using HaberApp.Core.Services;
using HaberApp.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Net;

namespace HaberApp.Services.Services
{
    public class ArticleService : ServiceBase<ArticleRequestDto, Article, ArticleResponseDto>, IArticlesService
    {
        private readonly IArticleRepository articleRepository;
        private readonly IClaudFlareService claudFlareService;


        public ArticleService(IArticleRepository articleRepository, IClaudFlareService claudFlareService = null)
        {
            this.articleRepository = articleRepository;
            this.claudFlareService = claudFlareService;

        }

        public override async Task<ResponseResult<ArticleResponseDto>> GetAllByQueryAsync(GetAllDataTypeRequestDto requestDto, Expression<Func<Article, bool>>? predicate = null, Func<IQueryable<Article>, IIncludableQueryable<Article, object>>? include = null, CancellationToken cancellationToken = default)
        {
            return await base.GetAllByQueryAsync(new()
            {
                DateSort = true,
                Count = requestDto.Count,
                Filters = requestDto.Filters,
                IsEnabled = requestDto.IsEnabled,
                Offset = requestDto.Offset,
                Sorting = requestDto.Sorting,
                GlobalFilter = requestDto.GlobalFilter,
                CategoryId = requestDto.CategoryId
            }, null, include, cancellationToken);
        }
        public async override Task<ResponseResult<ArticleResponseDto>> CreateAsync(ArticleRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.articleRepository.CreateAsync(new()
            {
                CategoryId = requestDto.CategoryId,
                Content = requestDto.Content,
                SeoDescription = requestDto.SeoDescription,
                SeoTitle = requestDto.SeoTitle,
                Title = requestDto.Title,
                SubDescription = requestDto.SubDescription,
                SourceUrl = requestDto.SourceUrl,
                ImageId = requestDto.ImageFile != null ? await this.claudFlareService.UploadImage(requestDto.ImageFile) : null
            }, cancellationToken);
            await this.unitOfWork.CommitAsync(cancellationToken);

            return new ResponseResult<ArticleResponseDto>()
            {
                Entity = this.mapper.Map<ArticleResponseDto>(result)
            };
        }

        public async override Task<ResponseResult<ArticleResponseDto>> UpdateAsync(int id, ArticleRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            //return base.UpdateAsync(id, requestDto, cancellationToken);

            var entity = this.mapper.Map<Article>(requestDto);

            if (requestDto.SourceUrl is null)
            {
                entity.SourceUrl = await this.appDbContext.Articles.AsNoTracking()
                    .Where(article => article.Id == id)
                    .Select(article => article.SourceUrl)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (requestDto.ImageFile is not null)
            {
                entity.ImageId = await this.claudFlareService.UploadImage(requestDto.ImageFile);
            }

            var result = await this.articleRepository.UpdateAsync(id, entity, cancellationToken);

            await this.unitOfWork.CommitAsync(cancellationToken);

            return new ResponseResult<ArticleResponseDto>()
            {
                Entity = this.mapper.Map<ArticleResponseDto>(result)
            };
        }

        public override async Task<ResponseResult<ArticleResponseDto>> FindAllAsync(Expression<Func<Article, bool>>? predicate, Func<IQueryable<Article>, IIncludableQueryable<Article, object>>? include = null, CancellationToken cancellationToken = default)
        {
            //return base.FindAllAsync(predicate, include, cancellationToken);

            var result = await this.appDbContext.Articles.AsNoTracking().Include(b => b.Category).Select(a => new Article()
            {
                Id = a.Id,
                Title = a.Title,
                SourceUrl = a.SourceUrl,
                Category = a.Category

            }).ToListAsync(cancellationToken);

            return new ResponseResult<ArticleResponseDto>()
            {
                Entities = this.mapper.Map<List<ArticleResponseDto>>(result)
            };
        }
        public async Task<ResponseResult<ArticleResponseDto>> GetLastNewsAsync(GetAllDataTypeRequestDto request, int? categoryId = null, CancellationToken cancellationToken = default)
        {

            var result = this.appDbContext.Articles.Include(a => a.Category).AsNoTracking().OrderByDescending(a => a.CreatedAt).AsQueryable();

            var count = 0;
            if (categoryId is not null)
            {
                result = result.Where(a => a.CategoryId == categoryId);
                count = await result.CountAsync(cancellationToken);

            }
            else
            {
                count = await result.CountAsync(cancellationToken);
                result = result
                       .GroupBy(a => a.CategoryId)
                       .Select(g => g
                           .OrderByDescending(a => a.CreatedAt)
                           .ThenByDescending(a => a.Id)
                           .First());
            }

            var lastResult = await result.Skip(request.Offset).Take(request.Count).ToListAsync(cancellationToken);
            return new ResponseResult<ArticleResponseDto>()
            {
                Entities = this.mapper.Map<List<ArticleResponseDto>>(lastResult),
                RowCount = count
            };
        }

        public async Task<ResponseResult<ArticleResponseDto>> GetPopularPosts(int? categoryId = null, CancellationToken token = default)
        {
            var result = this.appDbContext.Articles.AsNoTracking().AsQueryable();
            if (categoryId.HasValue)
            {
                result = result.Where(a => a.CategoryId == categoryId);
            }
            var lastResult = await result.OrderByDescending(a => a.ReadedCount)
                .ThenByDescending(a => a.CreatedAt)
                .Include(a => a.Category).Take(6).ToListAsync(token);

            return new ResponseResult<ArticleResponseDto>()
            {
                Entities = this.mapper.Map<List<ArticleResponseDto>>(lastResult)
            };
        }

        public async Task<ResponseResult<ArticleResponseDto>> GetMostPopularPost(int? categoryId = null, CancellationToken cancellationToken = default)
        {
            var result = this.appDbContext.Articles.AsNoTracking().Include(a => a.Category).AsQueryable();
            if (categoryId is not null)
            {
                result = result.Where(a => a.CategoryId == categoryId);
            }
            var lastResult = await result.OrderByDescending(a => a.ReadedCount).ThenByDescending(a => a.CreatedAt).FirstOrDefaultAsync(cancellationToken);
            return new ResponseResult<ArticleResponseDto>()
            {
                Entity = this.mapper.Map<ArticleResponseDto>(lastResult)
            };
        }

        public override async Task<ResponseResult<ArticleResponseDto>> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await this.appDbContext.Articles.AsNoTracking().Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (result == null)
            {
                throw new CustomException("error", "Not Found", HttpStatusCode.NotFound);
            }
            return new ResponseResult<ArticleResponseDto>()
            {
                Entity = this.mapper.Map<ArticleResponseDto>(result)
            };
        }

        public async Task<ResponseResult<ArticleResponseDto>> IncreaseReadCount(int id, CancellationToken cancellationToken = default)
        {
            var article = await this.articleRepository.GetByIdAsync(id, cancellationToken);
            if (article is null)
            {
                throw new CustomException("error", "Article Not Found", HttpStatusCode.NotFound);
            }
            article.ReadedCount++;
            await this.articleRepository.UpdateAsync(id, article, cancellationToken);
            await this.unitOfWork.CommitAsync(cancellationToken);
            return new ResponseResult<ArticleResponseDto>()
            {
                Entity = this.mapper.Map<ArticleResponseDto>(article)
            };
        }
    }
}
