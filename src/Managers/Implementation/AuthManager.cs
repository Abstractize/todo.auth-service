using Entities = Data.Entities;
using Data.Repositories.Contracts;
using Models;
using Services.Contracts;
using Managers.Contracts;

namespace Managers.Implementation;

internal class AuthManager(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IHasherService hasherService, ITokenService tokenService) : IAuthManager
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IHasherService _hasherService = hasherService;
    private readonly ITokenService tokenService = tokenService;

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        Entities.User user = await _userRepository.FindAsync(u => u.Email == loginRequest.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!_hasherService.Verify(loginRequest.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var (token, expiresAt) = await tokenService.GenerateAccessToken(user);
        var refreshToken = await tokenService.GenerateSecureRefreshToken();

        await _refreshTokenRepository.AddAsync(new Entities.RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });

        return new AuthResponse(token, refreshToken, expiresAt);
    }

    public async Task<AuthResponse> RefreshTokenAsync(TokenActionRequest request)
    {
        string refreshToken = request.RefreshToken;

        Entities.RefreshToken? existingRefreshToken = await _refreshTokenRepository
            .FindAsync(rt => rt.Token == refreshToken && rt.IsActive, includeUser: true) ??
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        Entities.User user = await _userRepository.FindAsync(x => x.Id == existingRefreshToken.UserId)
            ?? throw new UnauthorizedAccessException("User not found.");

        var (newToken, expiresAt) = await tokenService.GenerateAccessToken(user);
        var newRefreshToken = await tokenService.GenerateSecureRefreshToken();

        existingRefreshToken.RevokedAt = DateTime.UtcNow;
        existingRefreshToken.ReplacedByToken = newRefreshToken;

        await _refreshTokenRepository.UpdateAsync(existingRefreshToken);

        await _refreshTokenRepository.AddAsync(new Entities.RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });

        return new AuthResponse(newToken, newRefreshToken, expiresAt);
    }

    public async Task LogoutAsync(TokenActionRequest request)
    {
        string refreshToken = request.RefreshToken;

        Entities.RefreshToken? existingToken = await _refreshTokenRepository
            .FindAsync(x =>
                x.Token == request.RefreshToken &&
                x.RevokedAt == null &&
                x.ExpiresAt > DateTime.UtcNow
            );

        if (existingToken == null || !existingToken.IsActive)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        existingToken.RevokedAt = DateTime.UtcNow;

        await _refreshTokenRepository.UpdateAsync(existingToken);

    }
}
