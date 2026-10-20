using HaberApp.Core.Abstract;
using HaberApp.Core.Domains;
using HaberApp.Core.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace HaberApp.DataAccess.Contexts
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        private readonly IConfiguration configuration;

        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.configuration.GetConnectionString("SqlCon"), opt =>
            {
                opt.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);

            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(true, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var userId = ContextHelpers.CurrentUserId!;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedById = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Property(a => a.CreatedAt).IsModified = false;
                    entry.Property(a => a.CreatedById).IsModified = false;
                    entry.Entity.UpdatedById = userId;
                    //entry.Property(a => a.AddedById).IsModified = false;
                    //entry.Entity.UpdatedById = userId;
                }
            }

            var queuedGroups = ChangeTracker.Entries<IQueuable>()
                .Where(entry => entry.State == EntityState.Added)
                .GroupBy(entry => entry.Metadata.ClrType)
                .ToList();

            foreach (var group in queuedGroups)
            {
                var getLastQueueMethod = typeof(AppDbContext)
                    .GetMethod(nameof(GetLastQueueAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
                    .MakeGenericMethod(group.Key);
                var lastQueueTask = (Task<int?>)getLastQueueMethod.Invoke(this, new object[] { cancellationToken })!;
                var lastQueue = await lastQueueTask ?? 0;

                foreach (var entry in group)
                {
                    lastQueue = checked(lastQueue + 1);
                    entry.Entity.Queue = lastQueue;
                }
            }

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private Task<int?> GetLastQueueAsync<TEntity>(CancellationToken cancellationToken)
            where TEntity : class, IQueuable
        {
            return Set<TEntity>().MaxAsync(entity => (int?)entity.Queue, cancellationToken);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<CategoryNewsUrl> CategoryNewsUrls { get; set; }
    }
}
