namespace WavesBot.IAL
{
    using System.Threading.Tasks;
    using Configuration;
    using Data;
    using Repositories;
    using Utf8Json;
    using ViewLib.Repositories;

    public class UserProvider
    {
        private readonly IPropertiesRepository propertiesRepository;
        private readonly IUserDataRepository userRepository;

        public UserProvider(IPropertiesRepository propertiesRepository,
            IUserDataRepository userRepository)
        {
            this.propertiesRepository = propertiesRepository;
            this.userRepository = userRepository;
        }

        public async Task<UserData> GetAsync(long identifier)
        {
            var cache = await propertiesRepository.ReadAsync(identifier);

            if (cache.ContainsKey(ViewConstants.CachedDbUser))
                return JsonSerializer.Deserialize<UserData>((string) cache[ViewConstants.CachedDbUser]);

            var user = await userRepository.ReadAsync(identifier);

            return user;
        }

        public async Task CreateAsync(long identifier, string nickName)
        {
            var user = new UserData
            {
                Identifier = identifier,
                NickName = nickName ?? string.Empty
            };

            await userRepository.CreateAsync(identifier, user);
            await SyncUser(identifier, user);
        }

        public async Task UpdateNickNameAsync(long identifier, string nickName)
        {
            var user = await GetAsync(identifier);
            if (user != null && user.NickName != nickName)
            {
                user.NickName = nickName;

                await userRepository.UpdateAsync(user);
                await SyncUser(identifier, user);
            }
        }

        public async Task UpdateAsync(UserData user)
        {
            if (user == null) return;

            await userRepository.UpdateAsync(user);
            await SyncUser(user.Identifier, user);
        }

        /// <summary>
        /// Заносит пользователя в кеш
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="user"></param>
        private async Task SyncUser(long identifier, UserData user)
        {
            if (user == null) return;

            var properties = await propertiesRepository.ReadAsync(identifier);

            properties[ViewConstants.CachedDbUser] = JsonSerializer.ToJsonString(user);

            await propertiesRepository.CreateOrUpdateAsync(identifier, properties);
        }
    }
}