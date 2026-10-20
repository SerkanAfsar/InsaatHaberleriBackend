using HaberApp.Core.Abstract;
using HaberApp.Core.Domains;
using HaberApp.Core.Enums;

namespace HaberApp.Core.Services
{
    public class YapiComTrService : ISiteCategoryCrawler
    {
        public Task<Article> CreateArticleDetail(string url, NewsSites newsSite)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetNewsUrlList(string newsCategoryUrl, NewsSites newsSite)
        {
            throw new NotImplementedException();
        }
    }
}
