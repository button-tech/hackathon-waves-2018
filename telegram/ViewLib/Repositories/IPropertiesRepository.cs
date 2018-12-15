namespace ViewLib.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPropertiesRepository
    {
        /// <summary>
        ///     Создает или обновляет состояние хранилища Properties
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="propertiesDictionary"></param>
        /// <returns>UserId или ChatId для телеграмма</returns>
        Task CreateOrUpdateAsync(long identifier, Dictionary<string, object> propertiesDictionary);

        /// <summary>
        ///     Читает хранилище Properties
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>UserId или ChatId для телеграмма</returns>
        Task<Dictionary<string, object>> ReadAsync(long identifier);

        /// <summary>
        ///     Удаляет хранилище Properties
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task DeleteAsync(long identifier);
    }
}