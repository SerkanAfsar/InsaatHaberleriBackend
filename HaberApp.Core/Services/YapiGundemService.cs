using HaberApp.Core.Domains;
using HaberApp.Core.Enums;

namespace HaberApp.Core.Services
{
    public class YapiGundemService(HttpClient httpClient)
        : SiteCrawlerBase(httpClient, NewsSites.YapiGundem, "yapigundem.com")
    {
        public override async Task<List<string>> GetNewsUrlList(string newsCategoryUrl, NewsSites newsSite)
        {
            var (document, uri) = await LoadAsync(newsCategoryUrl, newsSite);
            return GetLinks(document, uri,
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' td-main-content ')]" +
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' td_module_wrap ')]" +
                "//h3[contains(concat(' ', normalize-space(@class), ' '), ' entry-title ')]/a",
                link => link.AbsolutePath.Trim('/').Length > 0 &&
                    !link.AbsolutePath.StartsWith("/kategori/", StringComparison.OrdinalIgnoreCase) &&
                    !link.AbsolutePath.StartsWith("/tag/", StringComparison.OrdinalIgnoreCase));
        }

        public override async Task<Article> CreateArticleDetail(string url, NewsSites newsSite)
        {
            var (document, uri) = await LoadAsync(url, newsSite);
            var title = FirstText(document, "//h1[contains(@class, 'tdb-title-text')]", "//h1[contains(@class, 'entry-title')]");
            var body = document.DocumentNode.SelectSingleNode(
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' tdb_single_content ')]" +
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' tdb-block-inner ')]");

            var lead = body?.SelectSingleNode("./p[1]");
            var subtitle = lead is not null ? Text(lead) : Meta(document, "description");
            if (lead is not null) lead.Remove();

            var article = ArticleFrom(title, subtitle, CleanContent(body, uri), uri);
            article.SourceImageUrl = FirstImageUrl(document, uri,
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' tdb_single_featured_image ')]//a[@href]",
                "//meta[@property='og:image']");
            return article;
        }
    }
}
