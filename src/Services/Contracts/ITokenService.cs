using Data.Entities;
using Models;

namespace Services.Contracts;

public interface ITokenService
{
    Task<(string Token, DateTime ExpiresAt)> GenerateAccessToken(User user);
    Task<string> GenerateSecureRefreshToken();
}