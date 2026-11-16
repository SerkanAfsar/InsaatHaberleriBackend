using HaberApp.Core.Domains;
using HaberApp.Core.Repositories;
using HaberApp.DataAccess.Contexts;

namespace HaberApp.DataAccess.Repositories
{
    public class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
