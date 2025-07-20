using Entities = Data.Entities;
using Data.Repositories.Contracts;
using Models;
using Models.Common.Exceptions;
using Services.Contracts;
using Services.Common.Identity;
using Managers.Contracts;
using Data.Entities;

namespace Managers.Implementation;

internal class AuthManager(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IHasherService<User> hasherService,
    ITokenService tokenService,
    IIdentityService identityService
) : IAuthManager
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IHasherService<User> _hasherService = hasherService;
    private readonly ITokenService tokenService = tokenService;
    private readonly IIdentityService _identityService = identityService;

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        // Validate the request

        User user = await _userRepository.FindAsync(u => u.Email == loginRequest.Email)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!_hasherService.Verify(user, loginRequest.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var (token, expiresAt) = await tokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.FullName,
            nameof(user.Role)
        );

        string refreshToken = await tokenService.GenerateSecureRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        });

        return new AuthResponse(token, refreshToken, expiresAt);
    }

    public async Task<AuthResponse> RefreshTokenAsync(TokenActionRequest request)
    {
        string refreshToken = request.RefreshToken;

        RefreshToken? existingRefreshToken = await _refreshTokenRepository
            .FindAsync(rt => rt.Token == refreshToken && rt.RevokedAtUtc == null && DateTime.UtcNow < rt.ExpiresAtUtc, includeUser: true) ??
            throw new UnauthorizedException("Invalid or expired refresh token.");

        User user = await _userRepository.FindAsync(x => x.Id == existingRefreshToken.UserId)
            ?? throw new UnauthorizedException("User not found.");

        (string newToken, DateTime expiresAt) = await tokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.FullName,
            nameof(user.Role)
        );

        string newRefreshToken = await tokenService.GenerateSecureRefreshToken();

        existingRefreshToken.RevokedAtUtc = DateTime.UtcNow;
        existingRefreshToken.ReplacedByToken = newRefreshToken;

        await _refreshTokenRepository.UpdateAsync(existingRefreshToken);

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        });

        return new AuthResponse(newToken, newRefreshToken, expiresAt);
    }

    public async Task LogoutAsync(TokenActionRequest request)
    {
        Guid? userId = _identityService.UserId ??
            throw new UnauthorizedException("User not authenticated.");

        string refreshToken = request.RefreshToken;

        RefreshToken? existingToken = await _refreshTokenRepository
            .FindAsync(x =>
                x.Token == request.RefreshToken &&
                x.RevokedAtUtc == null &&
                x.ExpiresAtUtc > DateTime.UtcNow &&
                x.UserId == userId
            );

        if (existingToken == null || !existingToken.IsActive())
            throw new UnauthorizedException("Invalid refresh token.");

        existingToken.RevokedAtUtc = DateTime.UtcNow;

        await _refreshTokenRepository.UpdateAsync(existingToken);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
    {
        // Validate the request

        User? existingUser = await _userRepository.FindAsync(u => u.Email == registerRequest.Email);

        if (existingUser != null)
            throw new BadRequestException("Email already exists.");

        User newUser = new()
        {
            Email = registerRequest.Email,
            FullName = registerRequest.FullName,
            Role = UserRole.User
        };

        newUser.PasswordHash = _hasherService.Hash(newUser, registerRequest.Password);

        await _userRepository.AddAsync(newUser);

        return await LoginAsync(new LoginRequest
        (
            Email: registerRequest.Email,
            Password: registerRequest.Password
        ));
    }
}
