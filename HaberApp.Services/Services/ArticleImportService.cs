using HaberApp.Core.Abstract;
using HaberApp.Core.Domains;
using HaberApp.Core.Enums;
using HaberApp.Core.Repositories;
using HaberApp.Core.Services;
using HaberApp.Core.UnitOfWorks;
using HaberApp.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HaberApp.Services.Services
{
    public class ArticleImportService(
        ICategoryNewsUrlRepository categoryNewsUrlRepository,
        IArticleRepository articleRepository,
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        ICloudflareImageService cloudflareImageService,
        InsaatDunyasiService insaatDeryasiService,
        YapiComTrService yapiComTrService,
        YapiGundemService yapiGundemService,
        ILogger<ArticleImportService> logger) : IArticleImportService
    {
        public async Task<int> ImportNewArticlesAsync(CancellationToken cancellationToken = default)
        {
            var sources = await categoryNewsUrlRepository.GetAllAsync()
                .OrderBy(source => source.Id)
                .ToListAsync(cancellationToken);
            var added = 0;

            foreach (var source in sources)
            {
                cancellationToken.ThrowIfCancellationRequested();
                List<string> urls;
                ISiteCategoryCrawler crawler;

                try
                {
                    crawler = ResolveCrawler(source.NewsSite);
                    urls = await crawler.GetNewsUrlList(source);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Kategori kaynak URL'si okunamadı: {SourceUrl}", source.Url);
                    continue;
                }

                foreach (var url in urls)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Article article;

                    try
                    {
                        article = await crawler.CreateArticleDetail(url, source);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        logger.LogError(ex, "Haber okunamadı: {ArticleUrl}", url);
                        continue;
                    }

                    Article? pendingArticle = null;
                    try
                    {
                        var title = article.Title.Trim();
                        article.Title = title;
                        var existingArticle = await articleRepository.GetAllAsync(
                                existing => existing.CategoryId == source.CategoryId && existing.Title == title)
                            .Select(existing => new { existing.Id, existing.ImageId })
                            .FirstOrDefaultAsync(cancellationToken);
                        if (existingArticle is not null && !string.IsNullOrWhiteSpace(existingArticle.ImageId))
                            continue;

                        if (existingArticle is not null && string.IsNullOrWhiteSpace(article.SourceImageUrl))
                            continue;

                        if (!string.IsNullOrWhiteSpace(article.SourceImageUrl))
                        {
                            try
                            {
                                article.ImageId = await cloudflareImageService.UploadFromUrlAsync(
                                    article.SourceImageUrl, cancellationToken);
                            }
                            catch (Exception ex) when (ex is not OperationCanceledException)
                            {
                                logger.LogError(ex, "Haber görseli yüklenemedi: {ArticleUrl}", url);
                                continue;
                            }
                        }

                        if (existingArticle is not null)
                        {
                            var trackedArticle = await articleRepository.GetByIdAsync(existingArticle.Id, cancellationToken);
                            if (trackedArticle is null) continue;
                            pendingArticle = trackedArticle;
                            trackedArticle.ImageId = article.ImageId;
                            trackedArticle.SourceUrl = article.SourceUrl;
                            await unitOfWork.CommitAsync(cancellationToken);
                            continue;
                        }

                        pendingArticle = article;
                        await articleRepository.CreateAsync(article, cancellationToken);
                        await unitOfWork.CommitAsync(cancellationToken);
                        added++;
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        if (pendingArticle is not null)
                            dbContext.Entry(pendingArticle).State = EntityState.Detached;

                        logger.LogError(ex, "Haber veritabanına kaydedilemedi: {ArticleUrl}. Sıradaki habere geçiliyor.", url);
                    }
                }
            }

            return added;
        }

        private ISiteCategoryCrawler ResolveCrawler(NewsSites site) => site switch
        {
            NewsSites.InsaatDeryasi => insaatDeryasiService,
            NewsSites.YapiComTr => yapiComTrService,
            NewsSites.YapiGundem => yapiGundemService,
            _ => throw new ArgumentOutOfRangeException(nameof(site), site, "Bilinmeyen haber kaynağı.")
        };
    }
}
