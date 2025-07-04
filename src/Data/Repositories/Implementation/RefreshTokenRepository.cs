using System.Linq.Expressions;
using Data.Context;
using Data.Entities;
using Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementation;

internal class RefreshTokenRepository(DatabaseContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token)
    {
        await context.RefreshTokens.AddAsync(token);

        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> FindAsync(Expression<Func<RefreshToken, bool>> filter, bool includeUser = false)
    {
        if (includeUser)
        {
            return await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(filter);
        }

        return await context.RefreshTokens.FirstOrDefaultAsync(filter);
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        context.RefreshTokens.Update(token);
        await context.SaveChangesAsync();
    }
}