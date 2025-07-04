using Data.Context;
using Data.Repositories.Contracts;
using Data.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNet.Identity;

namespace Data
{
    public static class IServiceCollectionEx
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string? connectionString = null)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    options.UseInMemoryDatabase("DefaultDatabase");
                }
                else
                {
                    options.UseNpgsql(connectionString);
                }
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }

        public static async Task SeedDatabaseAsync(this IHost host, string email, string password)
        {
            using IServiceScope scope = host.Services.CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            try
            {
                DatabaseContext context = services.GetRequiredService<DatabaseContext>();
                IPasswordHasher passwordHasher = services.GetRequiredService<IPasswordHasher>();

                await context.Database.EnsureCreatedAsync();

                if (!context.Users.Any())
                {
                    Entities.User admin = new()
                    {
                        Id = Guid.NewGuid(),
                        FullName = "Admin User",
                        Email = email,
                        PasswordHash = passwordHasher.HashPassword(password),
                        Role = Entities.UserRole.Administrator,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Users.AddRange(admin);
                    await context.SaveChangesAsync();

                    Console.WriteLine($"Database seeded with admin user {email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Error seeding the database.");
            }
        }
    }
}