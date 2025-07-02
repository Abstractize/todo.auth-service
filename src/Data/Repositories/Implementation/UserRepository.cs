using Data.Context;
using Data.Entities;
using Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories.Implementation
{
    internal class UserRepository(DatabaseContext context) : IUserRepository
    {
        private readonly DatabaseContext _context = context;

        public async Task<User?> FindAsync(Expression<Func<User, bool>> filter)
            => await _context.Users.FirstOrDefaultAsync(filter);
    }
}
