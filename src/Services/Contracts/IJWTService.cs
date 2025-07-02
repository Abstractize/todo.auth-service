using Data.Entities;
using Models;

namespace Services.Contracts;

public interface IJWTService
{
    AuthResponse GenerateToken(User user);
}