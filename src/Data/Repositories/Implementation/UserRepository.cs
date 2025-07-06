using Data.Common.Repositories.Implementation;
using Data.Context;
using Data.Entities;
using Data.Repositories.Contracts;

namespace Data.Repositories.Implementation
{
    internal class UserRepository(DatabaseContext context)
        : BaseRepository<User, DatabaseContext>(context),
        IUserRepository
    { }
}
