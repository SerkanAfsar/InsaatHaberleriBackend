using HaberApp.Core.Domains;
using HaberApp.Core.Repositories;
using HaberApp.DataAccess.Contexts;

namespace HaberApp.DataAccess.Repositories
{
    public class ArticleRepository(AppDbContext appDbContext) : RepositoryBase<Article>(appDbContext), IArticleRepository
    {
    }
}
