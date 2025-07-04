using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data.Entities;
using Microsoft.IdentityModel.Tokens;
using Models;
using Services.Contracts;

namespace Services.Implementation;

internal class TokenService(string audience, string issuer, string jwtKey) : ITokenService
{
    private const int EXPIRATION_TIME_IN_HOURS = 1;

    private readonly string _audience = audience;
    private readonly string _issuer = issuer;
    private readonly string _jwtKey = jwtKey;

    public Task<(string Token, DateTime ExpiresAt)> GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.Now.AddHours(EXPIRATION_TIME_IN_HOURS);

        Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, nameof(user.Role))
        ];

        JwtSecurityToken token = new(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult((Token: jwtToken, ExpiresAt: expiresAt));
    }

    public Task<string> GenerateSecureRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Task.FromResult(Convert.ToBase64String(randomBytes));
    }
}
