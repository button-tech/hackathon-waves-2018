namespace ViewLib.Repositories
{
    using System;
    using System.Threading.Tasks;

    public interface INavigationRepository
    {
        /// <summary>
        ///     Создает или обновляет стек навигации
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="navigationArray"></param>
        /// <returns></returns>
        Task CreateOrUpdateAsync(long identifier, Type[] navigationArray);

        /// <summary>
        ///     Читает стек навигации
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<Type[]> ReadAsync(long identifier);

        /// <summary>
        ///     Удаляет стек навигации
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task DeleteAsync(long identifier);
    }
}