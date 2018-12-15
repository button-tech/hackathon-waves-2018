namespace ViewLib.Services
{
    using System.Threading.Tasks;
    using Data;

    //todo: Разделить на несколько разных интерфейсов
    public interface ICache
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
        ///     Заносит или обновляет TelegramGuidStamp в кеш на 10 минут
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="guidStamp"></param>
        /// <returns></returns>
        Task CreateOrUpdateGuidAsync(string guid, TelegramGuidStamp guidStamp);

        /// <summary>
        ///     Читает хранилище Properties из кеша
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<PropertiesStamp> ReadPropertiesAsync(long identifier);

        /// <summary>
        ///     Заносит или обновляет хранилище Properties в кеш на 15 минут
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        Task CreateOrUpdatePropertiesAsync(long identifier, PropertiesStamp props);

        /// <summary>
        ///     Удаляет хранилище Properties из кеша
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task DeletePropertiesAsync(long identifier);

        /// <summary>
        ///     Читает стек навигации из кеша
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<NavigationStamp> ReadNavigationAsync(long identifier);

        /// <summary>
        ///     Заносит или обновляет стек навигации в кеш на 15 минут
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="navigationCache"></param>
        /// <returns></returns>
        Task CreateOrUpdateNavigationAsync(long identifier, NavigationStamp navigationCache);

        /// <summary>
        ///     Удаляет стек навигации из кеша
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task DeleteNavigationAsync(long identifier);

        /// <summary>
        ///     Читает курсы из кеша
        /// </summary>
        /// <returns></returns>
        Task<CourseStamp> ReadCoursesAsync();

        /// <summary>
        ///     Заносит в кеш курсы на 6 минут
        /// </summary>
        /// <param name="courseStamp"></param>
        /// <returns></returns>
        Task CreateOrUpdateCoursesAsync(CourseStamp courseStamp);
    }
}