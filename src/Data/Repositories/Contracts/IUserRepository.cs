using Data.Entities;
using System.Linq.Expressions;

namespace Data.Repositories.Contracts;

public interface IUserRepository
{
    Task<User?> FindAsync(Expression<Func<User, bool>> filter);
}