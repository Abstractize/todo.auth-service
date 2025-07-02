using Managers.Contracts;

namespace API.Endpoints
{
    public static class AuthEndpoint
    {
        public static void MapAuthEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/login", (IAuthManager authManager) => authManager.LoginAsync);
        }
    }
}
