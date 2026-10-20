namespace HaberApp.Core.Services
{
    public interface IArticleImportService
    {
        Task<int> ImportNewArticlesAsync(CancellationToken cancellationToken = default);
    }
}
