namespace WavesBot.Repositories
{
    using System.Threading.Tasks;
    using Data;

    public interface IUserDataRepository
    {
        Task<UserData> ReadAsync(long identifier);

        Task<UserData[]> ReadAsync(long[] identifiers);

        Task<UserData> ReadAsync(string nickName);

        Task CreateAsync(long identifier, UserData user);

        Task UpdateAsync(UserData user);
    }
}