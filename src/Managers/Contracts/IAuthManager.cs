using Models;

namespace Managers.Contracts;

public interface IAuthManager
{
    Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
}