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
                [AllowAnonymous] (IAuthManager authManager, [FromBody] LoginRequest loginRequest)
                    => authManager.LoginAsync(loginRequest));

            app.MapPost("/api/auth/refresh-token",
                [AllowAnonymous] (IAuthManager authManager, [FromBody] TokenActionRequest request)
                    => authManager.RefreshTokenAsync(request));

            app.MapPost("/api/auth/logout",
                [Authorize] (IAuthManager authManager, [FromBody] TokenActionRequest request)
                    => authManager.LogoutAsync(request));

            app.MapPost("/api/auth/register",
                [AllowAnonymous] (IAuthManager authManager, [FromBody] RegisterRequest registerRequest)
                    => authManager.RegisterAsync(registerRequest));
        }
    }
}
