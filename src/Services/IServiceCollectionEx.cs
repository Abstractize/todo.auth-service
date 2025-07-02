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
            services.AddScoped<IHasherService>(x => new HasherService(new PasswordHasher()));
            services.AddScoped<IJWTService>(x => new JWTService(audience, issuer, jwtKey));

            return services;
        }
    }
}
