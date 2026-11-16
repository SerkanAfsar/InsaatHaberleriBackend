using System.Text.RegularExpressions;
using HaberApp.Core.Domains;
using HaberApp.Core.Enums;

namespace HaberApp.Core.Services
{
    // Sınıf adı mevcut mimariyle uyumlu; kaynak site İnşaat Deryası'dır.
    public class InsaatDunyasiService(HttpClient httpClient)
        : SiteCrawlerBase(httpClient, NewsSites.InsaatDeryasi, "insaatderyasi.com")
    {
        private static readonly Regex ArticlePath = new(@"-\d+h\.htm$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public override async Task<List<string>> GetNewsUrlList(string newsCategoryUrl, NewsSites newsSite)
        {
            var (document, uri) = await LoadAsync(newsCategoryUrl, newsSite);
            return GetLinks(document, uri,
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' page-content ')]" +
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' box-news ')]" +
                "//a[contains(concat(' ', normalize-space(@class), ' '), ' c ')]",
                link => ArticlePath.IsMatch(link.AbsolutePath));
        }

        public override async Task<Article> CreateArticleDetail(string url, NewsSites newsSite)
        {
            var (document, uri) = await LoadAsync(url, newsSite);
            var title = FirstText(document, "//h1[contains(@class, 'content-title')]");
            var subtitle = FirstText(document, "//h2[contains(@class, 'content-description')]");
            var body = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', normalize-space(@class), ' '), ' text-content ')]");
            var article = ArticleFrom(title, subtitle, CleanContent(body, uri), uri);
            article.SourceImageUrl = FirstImageUrl(document, uri,
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' imgc ')]/img",
                "//meta[@property='og:image']");
            return article;
        }
    }
}
