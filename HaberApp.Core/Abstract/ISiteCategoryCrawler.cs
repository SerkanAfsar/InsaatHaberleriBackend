using HaberApp.Core.Domains;
using HaberApp.Core.Enums;

namespace HaberApp.Core.Abstract
{
    public interface ISiteCategoryCrawler
    {
        Task<List<string>> GetNewsUrlList(string newsCategoryUrl, NewsSites newsSite);

        Task<Article> CreateArticleDetail(string url, NewsSites newsSite);
    }
}
