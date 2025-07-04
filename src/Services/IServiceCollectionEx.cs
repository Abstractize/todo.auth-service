using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using Services.Contracts;
using Services.Implementation;

namespace Services
{
    public static class IServiceCollectionEx
    {
        public static IServiceCollection AddServices(this IServiceCollection services, string audience, string issuer, string jwtKey)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IHasherService, HasherService>();
            services.AddScoped<ITokenService>(x => new TokenService(audience, issuer, jwtKey));

            return services;
        }
    }
}
