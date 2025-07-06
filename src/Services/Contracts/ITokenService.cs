namespace Services.Contracts;

public interface ITokenService
{
    Task<(string Token, DateTime ExpiresAt)> GenerateAccessToken(Guid userId, string email, string fullName, string role);
    Task<string> GenerateSecureRefreshToken();
}