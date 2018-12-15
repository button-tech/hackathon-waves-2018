namespace ViewLib.Repositories
{
    using System.Threading.Tasks;
    using Data;

    public interface ICoursesRepository
    {
        /// <summary>
        ///     Создает или обновляет CourseStamp в кеше
        /// </summary>
        /// <param name="courses">Курсы валют</param>
        /// <returns></returns>
        Task CreateOrUpdateAsync(CourseStamp courses);

        /// <summary>
        ///     Читает CourseStamp из кеша
        /// </summary>
        Task<CourseStamp> ReadAsync();
    }
}