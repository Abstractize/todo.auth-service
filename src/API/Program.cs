using API.Endpoints;
using API.Common.Middlewares;
using Data;
using Managers;
using Services;
using Data.Entities;
using Services.Common.Identity;

namespace API;

public class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string JWT_ISSUER = builder.Configuration.GetValue<string>(nameof(JWT_ISSUER))!;
        string JWT_AUDIENCE = builder.Configuration.GetValue<string>(nameof(JWT_AUDIENCE))!;
        string JWT_KEY = builder.Configuration.GetValue<string>(nameof(JWT_KEY))!;

        string? SQL_CONNECTION_STRING = builder.Configuration
            .GetValue<string>(nameof(SQL_CONNECTION_STRING));

        builder.Services.AddDataLayer(SQL_CONNECTION_STRING);
        builder.Services.AddIdentityService();
        builder.Services.AddServices<User>(JWT_AUDIENCE, JWT_ISSUER, JWT_KEY);
        builder.Services.AddManagers();

        builder.Services.AddCamelCaseJson();

        builder.Services.AddAuthConfiguration(JWT_ISSUER, JWT_AUDIENCE, JWT_KEY);

        WebApplication app = builder.Build();

        app.UseJsonExceptionHandler();
        app.UseAuth();

        app.MapAuthEndpoint();

        app.Run();
    }
}