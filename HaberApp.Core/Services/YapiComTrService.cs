using HaberApp.Core.Domains;
using HaberApp.Core.Enums;

namespace HaberApp.Core.Services
{
    public class YapiComTrService(HttpClient httpClient)
        : SiteCrawlerBase(httpClient, NewsSites.YapiComTr, "yapi.com.tr")
    {
        public override async Task<List<string>> GetNewsUrlList(string newsCategoryUrl, NewsSites newsSite)
        {
            var (document, uri) = await LoadAsync(newsCategoryUrl, newsSite);
            var links = GetLinks(document, uri,
                "//main//article//h2/a | //main//article//h3/a | " +
                "//main//*[contains(@class, 'news') or contains(@class, 'card')]//h2/a | " +
                "//main//*[contains(@class, 'news') or contains(@class, 'card')]//h3/a | " +
                "//article//h2/a | //article//h3/a",
                IsArticle);

            return links.Count > 0 ? links : GetLinks(document, uri, "//h2/a | //h3/a", IsArticle);
        }

        public override async Task<Article> CreateArticleDetail(string url, NewsSites newsSite)
        {
            var (document, uri) = await LoadAsync(url, newsSite);
            var title = FirstText(document, "//article//h1", "//main//h1", "//h1");
            var subtitle = FirstText(document,
                "//article//*[contains(@class, 'summary') or contains(@class, 'spot') or contains(@class, 'lead')]",
                "//main//*[contains(@class, 'summary') or contains(@class, 'spot') or contains(@class, 'lead')]");
            if (string.IsNullOrWhiteSpace(subtitle)) subtitle = Meta(document, "description");

            var body = document.DocumentNode.SelectSingleNode(
                "//article//*[contains(@class, 'article-content') or contains(@class, 'news-content') or contains(@class, 'detail-content') or contains(@class, 'entry-content') or contains(@class, 'text-content') or contains(@class, 'body-content')]")
                ?? document.DocumentNode.SelectSingleNode("//article//*[contains(@class, 'content')]")
                ?? document.DocumentNode.SelectSingleNode("//article");

            var article = ArticleFrom(title, subtitle, CleanContent(body, uri), uri);
            article.SourceImageUrl = FirstImageUrl(document, uri,
                "//meta[@property='og:image']",
                "//article//img[contains(@class, 'featured') or contains(@class, 'cover')]",
                "//article//img");
            return article;
        }

        private static bool IsArticle(Uri link)
        {
            var path = link.AbsolutePath.Trim('/');
            return path.Length > 8 && !path.Contains('/') ||
                link.AbsolutePath.StartsWith("/haberler/", StringComparison.OrdinalIgnoreCase) &&
                link.AbsolutePath.EndsWith(".html", StringComparison.OrdinalIgnoreCase);
        }
    }
}
