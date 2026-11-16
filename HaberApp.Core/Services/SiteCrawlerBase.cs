using System.Net;
using System.Text.RegularExpressions;
using HaberApp.Core.Abstract;
using HaberApp.Core.Domains;
using HaberApp.Core.Enums;
using HtmlAgilityPack;

namespace HaberApp.Core.Services
{
    public abstract class SiteCrawlerBase(HttpClient httpClient, NewsSites site, string host) : ISiteCategoryCrawler
    {
        private static readonly Regex Whitespace = new(@"\s+", RegexOptions.Compiled);
        private static readonly Regex AdToken = new(@"(^|[\s_-])(ad|ads|advert|advertisement|reklam|banner|sponsor|sponsored|promo)(?=$|[\s_-])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly HashSet<string> RemovedTags = new(StringComparer.OrdinalIgnoreCase)
        {
            "script", "style", "noscript", "iframe", "form", "button", "svg", "canvas", "video", "audio", "object", "embed"
        };
        private static readonly HashSet<string> KeptTags = new(StringComparer.OrdinalIgnoreCase)
        {
            "p", "div", "span", "br", "strong", "b", "em", "i", "u", "h2", "h3", "h4", "h5",
            "ul", "ol", "li", "blockquote", "a", "img", "figure", "figcaption", "table", "thead", "tbody", "tr", "th", "td"
        };

        protected readonly HttpClient HttpClient = httpClient;

        protected async Task<(HtmlDocument Document, Uri Uri)> LoadAsync(string url, NewsSites newsSite)
        {
            if (newsSite != site)
                throw new ArgumentException($"{GetType().Name} yalnızca {site} kaynağı için kullanılabilir.", nameof(newsSite));

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp ||
                !IsSiteHost(uri.Host))
                throw new ArgumentException($"URL {host} alan adına ait olmalıdır.", nameof(url));

            using var response = await HttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return (document, uri);
        }

        protected List<string> GetLinks(HtmlDocument document, Uri categoryUri, string xpath, Func<Uri, bool> isArticle)
        {
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var link in document.DocumentNode.SelectNodes(xpath) ?? Enumerable.Empty<HtmlNode>())
            {
                var href = WebUtility.HtmlDecode(link.GetAttributeValue("href", ""));
                if (!Uri.TryCreate(categoryUri, href, out var uri) ||
                    uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp ||
                    !IsSiteHost(uri.Host) || !isArticle(uri))
                    continue;

                var clean = new UriBuilder(uri) { Query = "", Fragment = "" }.Uri.AbsoluteUri;
                if (seen.Add(clean)) result.Add(clean);
            }

            return result;
        }

        protected static string Text(HtmlNode? node) =>
            Whitespace.Replace(WebUtility.HtmlDecode(node?.InnerText ?? ""), " ").Trim();

        protected static string FirstText(HtmlDocument document, params string[] xpaths)
        {
            foreach (var xpath in xpaths)
            {
                var value = Text(document.DocumentNode.SelectSingleNode(xpath));
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }
            return "";
        }

        protected static string Meta(HtmlDocument document, string key)
        {
            var node = document.DocumentNode.SelectSingleNode($"//meta[@name='{key}' or @property='{key}']");
            return Whitespace.Replace(WebUtility.HtmlDecode(node?.GetAttributeValue("content", "") ?? ""), " ").Trim();
        }

        protected static string? FirstImageUrl(HtmlDocument document, Uri pageUri, params string[] xpaths)
        {
            foreach (var xpath in xpaths)
            {
                var node = document.DocumentNode.SelectSingleNode(xpath);
                if (node is null) continue;

                var value = node.GetAttributeValue("content", "");
                if (string.IsNullOrWhiteSpace(value)) value = node.GetAttributeValue("href", "");
                if (string.IsNullOrWhiteSpace(value)) value = node.GetAttributeValue("data-src", "");
                if (string.IsNullOrWhiteSpace(value)) value = node.GetAttributeValue("src", "");
                var imageUrl = SafeUrl(pageUri, value);
                if (imageUrl is not null) return imageUrl;
            }

            return null;
        }

        protected static Article ArticleFrom(string title, string subtitle, string content, Uri pageUri)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
                throw new InvalidDataException("Haber başlığı veya içeriği kaynak sayfada bulunamadı.");

            return new Article
            {
                Title = title,
                SubDescription = subtitle,
                Content = content,
                SeoTitle = title,
                SeoDescription = subtitle,
                SourceUrl = pageUri.AbsoluteUri
            };
        }

        protected static string CleanContent(HtmlNode? source, Uri pageUri)
        {
            if (source is null) return "";

            var body = source.CloneNode(true);
            foreach (var node in body.Descendants().ToList().AsEnumerable().Reverse())
            {
                var name = node.Name;
                var marker = node.GetAttributeValue("class", "") + " " + node.GetAttributeValue("id", "");
                if (RemovedTags.Contains(name) || AdToken.IsMatch(marker) ||
                    name.Equals("img", StringComparison.OrdinalIgnoreCase) &&
                    (node.GetAttributeValue("width", "") == "1" || node.GetAttributeValue("height", "") == "1"))
                {
                    node.Remove();
                    continue;
                }

                if (node.NodeType != HtmlNodeType.Element) continue;
                if (!KeptTags.Contains(name))
                {
                    if (node.ParentNode is null) continue;
                    foreach (var child in node.ChildNodes.ToList())
                        node.ParentNode.InsertBefore(child, node);
                    node.Remove();
                    continue;
                }

                string? link = null;
                string? image = null;
                string? alt = null;
                if (name.Equals("a", StringComparison.OrdinalIgnoreCase))
                    link = SafeUrl(pageUri, node.GetAttributeValue("href", ""));
                if (name.Equals("img", StringComparison.OrdinalIgnoreCase))
                {
                    image = SafeUrl(pageUri, node.GetAttributeValue("data-src", node.GetAttributeValue("src", "")));
                    alt = node.GetAttributeValue("alt", "");
                }

                node.Attributes.RemoveAll();
                if (link is not null) node.SetAttributeValue("href", link);
                if (image is not null) node.SetAttributeValue("src", image);
                if (!string.IsNullOrWhiteSpace(alt)) node.SetAttributeValue("alt", alt);
            }

            return body.InnerHtml.Trim();
        }

        private bool IsSiteHost(string candidate) =>
            candidate.Equals(host, StringComparison.OrdinalIgnoreCase) ||
            candidate.Equals("www." + host, StringComparison.OrdinalIgnoreCase);

        private static string? SafeUrl(Uri pageUri, string value)
        {
            if (!Uri.TryCreate(pageUri, WebUtility.HtmlDecode(value), out var uri) ||
                uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
                return null;
            return uri.AbsoluteUri;
        }

        public abstract Task<List<string>> GetNewsUrlList(string newsCategoryUrl, NewsSites newsSite);
        public abstract Task<Article> CreateArticleDetail(string url, NewsSites newsSite);

        public Task<List<string>> GetNewsUrlList(CategoryNewsUrl categorySourceUrl)
        {
            ArgumentNullException.ThrowIfNull(categorySourceUrl);
            return GetNewsUrlList(categorySourceUrl.Url, categorySourceUrl.NewsSite);
        }

        public async Task<Article> CreateArticleDetail(string url, CategoryNewsUrl categorySourceUrl)
        {
            ArgumentNullException.ThrowIfNull(categorySourceUrl);
            var article = await CreateArticleDetail(url, categorySourceUrl.NewsSite);
            article.CategoryId = categorySourceUrl.CategoryId;
            return article;
        }
    }
}
