namespace Services.Contracts
{
    public interface IHasherService<TUser>
        where TUser : class
    {
        string Hash(TUser user, string password);
        bool Verify(TUser user, string password, string hash);
    }
}