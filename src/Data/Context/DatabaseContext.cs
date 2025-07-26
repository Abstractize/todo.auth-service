using Data.Common.Context;
using Data.Entities;
using Data.ModelBuilders;
using Microsoft.EntityFrameworkCore;
using Services.Common.Identity;

namespace Data.Context
{
    public class DatabaseContext(
        IIdentityService identityService,
        DbContextOptions<DatabaseContext> options
    ) : BaseContext<DatabaseContext>(identityService, options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.BuildUserModel();
            modelBuilder.BuildRefreshTokenModel();

            base.OnModelCreating(modelBuilder);
        }
    }
}
