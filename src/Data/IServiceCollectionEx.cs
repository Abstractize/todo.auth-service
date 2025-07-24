using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Data.Repositories;
using Data.Common.Extensions;
using Data.Common.Context;

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

            services.AddRepositories();

            return services;
        }

        public static void MigrateAtStartup(this IHost host, bool applyMigrations = true)
        {
            if (applyMigrations)
                host.ApplyMigrations<DatabaseContext>();
        }

    }
}