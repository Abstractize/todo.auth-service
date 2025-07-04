using Models;

namespace Managers.Contracts;

public interface IAuthManager
{
    Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
    Task<AuthResponse> RefreshTokenAsync(TokenActionRequest request);
    Task LogoutAsync(TokenActionRequest request);
}