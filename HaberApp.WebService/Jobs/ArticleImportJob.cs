using HaberApp.Core.Services;
using Hangfire;

namespace HaberApp.WebService.Jobs;

public class ArticleImportJob(IArticleImportService articleImportService, ILogger<ArticleImportJob> logger)
{
    [DisableConcurrentExecution(timeoutInSeconds: 3600)]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var added = await articleImportService.ImportNewArticlesAsync(cancellationToken);
        logger.LogInformation("Günlük haber aktarımı tamamlandı. Eklenen haber sayısı: {Added}", added);
    }
}
