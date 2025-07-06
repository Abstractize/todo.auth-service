using Microsoft.AspNetCore.Identity;
using Services.Contracts;

namespace Services.Implementation;

internal class HasherService<TUser>(IPasswordHasher<TUser> passwordHasher) : IHasherService<TUser>
    where TUser : class
{
    private readonly IPasswordHasher<TUser> _passwordHasher = passwordHasher;

    public string Hash(TUser user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool Verify(TUser user, string password, string hash)
    {
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, hash, password);

        return result switch
        {
            PasswordVerificationResult.Success => true,
            PasswordVerificationResult.Failed => false,
            PasswordVerificationResult.SuccessRehashNeeded => true,
            _ => false,
        };
    }
}
