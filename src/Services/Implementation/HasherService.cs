using Microsoft.AspNet.Identity;
using Services.Contracts;

namespace Services.Implementation;

internal class HasherService(PasswordHasher passwordHasher) : IHasherService
{
    private readonly PasswordHasher _passwordHasher = passwordHasher;

    public string Hash(string password)
    {
        return _passwordHasher.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(hash, password);

        return result switch
        {
            PasswordVerificationResult.Success => true,
            PasswordVerificationResult.Failed => false,
            PasswordVerificationResult.SuccessRehashNeeded => true,
            _ => false,
        };
    }
}
