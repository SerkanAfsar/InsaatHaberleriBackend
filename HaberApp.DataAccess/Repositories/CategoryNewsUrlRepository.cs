using HaberApp.Core.Domains;
using HaberApp.Core.Repositories;
using HaberApp.DataAccess.Contexts;

namespace HaberApp.DataAccess.Repositories
{
    public class CategoryNewsUrlRepository(AppDbContext appDbContext) : RepositoryBase<CategoryNewsUrl>(appDbContext), ICategoryNewsUrlRepository
    {

    }
}
