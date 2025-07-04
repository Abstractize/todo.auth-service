using Managers.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Endpoints
{
    public static class AuthEndpoint
    {
        public static void MapAuthEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/login",
                (IAuthManager authManager, [FromBody] LoginRequest loginRequest)
                    => authManager.LoginAsync(loginRequest));

            app.MapPost("/api/auth/refresh-token",
                (IAuthManager authManager, [FromBody] TokenActionRequest request)
                    => authManager.RefreshTokenAsync(request));

            app.MapPost("/api/auth/logout",
                (IAuthManager authManager, [FromBody] TokenActionRequest request)
                    => authManager.LogoutAsync(request));
        }
    }
}
