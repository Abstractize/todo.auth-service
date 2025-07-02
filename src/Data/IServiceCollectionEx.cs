using Data.Context;
using Data.Repositories.Contracts;
using Data.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

            return services;
        }
    }
}
