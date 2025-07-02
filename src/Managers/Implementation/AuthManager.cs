using Entities = Data.Entities;
using Data.Repositories.Contracts;
using Models;
using Services.Contracts;
using Managers.Contracts;

namespace Managers.Implementation
{
    internal class AuthManager(IUserRepository userRepository, IHasherService hasherService, IJWTService jwtService) : IAuthManager
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IHasherService _hasherService = hasherService;
        private readonly IJWTService jWTService = jwtService;

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userRepository.FindAsync(u => u.Email == loginRequest.Email)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");

            if (!_hasherService.Verify(loginRequest.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return jWTService.GenerateToken(user);
        }
    }
}
