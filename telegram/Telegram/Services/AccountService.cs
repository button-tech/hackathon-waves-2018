namespace WavesBot.Services
{
    using System.Threading.Tasks;
    using Data;
    using IAL;

    public class AccountService
    {
        private readonly UserProvider userProvider;

        public AccountService(UserProvider userProvider)
        {
            this.userProvider = userProvider;
        }

        /// <summary>
        ///     Создает пользователя
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public async Task CreateTelegramUser(long identifier, string nickName)
        {
            await userProvider.CreateAsync(identifier, nickName);
        }

        /// <summary>
        /// Обновляет ник юзера, если он изменился
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public async Task UpdateNickNameAsync(long identifier, string nickName)
        {
            await userProvider.UpdateNickNameAsync(identifier, nickName);
        }

        /// <summary>
        /// Проверяет есть ли пользователь в системе
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task<bool> IsExistAsync(long identifier)
        {
            var user = await ReadUser(identifier);
            return user != null;
        }

        /// <summary>
        /// Проверяет есть ли у пользователя публичные ключи
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task<bool> IsRegisteredAsync(long identifier)
        {
            var user = await ReadUser(identifier);
            return !string.IsNullOrEmpty(user?.WavesAddress);
        }

        /// <summary>
        /// Получает пользователя
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task<UserData> ReadUser(long identifier)
        {
            return await userProvider.GetAsync(identifier);
        }

        /// <summary>
        /// Удаляет аккаунт пользователя
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task DeleteUserAccount(long identifier)
        {
            var user = await userProvider.GetAsync(identifier);

            user.WavesAddress = default(string);

            await userProvider.UpdateAsync(user);
        }
    }
}