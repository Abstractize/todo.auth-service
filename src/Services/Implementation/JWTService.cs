using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data.Entities;
using Microsoft.IdentityModel.Tokens;
using Models;
using Services.Contracts;

namespace Services.Implementation
{
    internal class JWTService(string audience, string issuer, string jwtKey) : IJWTService
    {
        private const int EXPIRATION_TIME_IN_HOURS = 1;

        private readonly string _audience = audience;
        private readonly string _issuer = issuer;
        private readonly string _jwtKey = jwtKey;

        public AuthResponse GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(EXPIRATION_TIME_IN_HOURS);

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
                expires: expires,
                signingCredentials: creds
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse(Token: jwtToken, ExpiresAt: expires);
        }
    }
}
