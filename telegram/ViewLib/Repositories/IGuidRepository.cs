namespace ViewLib.Repositories
{
    using System.Threading.Tasks;
    using Data;

    public interface IGuidRepository
    {
        /// <summary>
        ///     Читает TelegramGuidStamp из кеша
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<TelegramGuidStamp> ReadGuidAsync(string guid);

        /// <summary>
        ///     Удаляет TelegramGuidStamp
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task DeleteGuidAsync(string guid);

        /// <summary>
        ///     Заносит или обновляет TelegramGuidStamp в кеш
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="guidStamp"></param>
        /// <returns></returns>
        Task CreateOrUpdateGuidAsync(string guid, TelegramGuidStamp guidStamp);
    }
}