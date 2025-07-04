using Data.Entities;
using Data.ModelBuilders;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    internal class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.BuildUserModel();
            modelBuilder.BuildRefreshTokenModel();
        }
    }
}
