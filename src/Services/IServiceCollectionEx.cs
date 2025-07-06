using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Services.Contracts;
using Services.Implementation;

namespace Services
{
    public static class IServiceCollectionEx
    {
        public static IServiceCollection AddServices<TUser>(this IServiceCollection services, string audience, string issuer, string jwtKey)
            where TUser : class
        {
            services.AddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.AddScoped<IHasherService<TUser>, HasherService<TUser>>();
            services.AddScoped<ITokenService>(x => new TokenService(audience, issuer, jwtKey));

            return services;
        }
    }
}
