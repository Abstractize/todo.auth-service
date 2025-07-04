using System.Linq.Expressions;
using Data.Entities;

namespace Data.Repositories.Contracts;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> FindAsync(Expression<Func<RefreshToken, bool>> filter, bool includeUser = false);
    Task UpdateAsync(RefreshToken token);
}
