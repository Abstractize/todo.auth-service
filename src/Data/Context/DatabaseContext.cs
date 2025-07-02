using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    internal class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.HasIndex(entity => entity.Email)
                    .IsUnique();

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasConversion<string>();
            });
        }
    }
}
