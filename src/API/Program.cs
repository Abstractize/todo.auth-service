using API.Endpoints;
using Data;
using Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.Text.Json;

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
        builder.Services.AddServices(JWT_AUDIENCE, JWT_ISSUER, JWT_KEY);
        builder.Services.AddManagers();

        builder.Services.Configure<JsonOptions>(opt =>
        {
            opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opt.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JWT_ISSUER,
                    ValidAudience = JWT_AUDIENCE,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JWT_KEY))
                };
            }
        );

        builder.Services.AddAuthorization();


        WebApplication app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAuthEndpoint();

        app.Run();
    }
}