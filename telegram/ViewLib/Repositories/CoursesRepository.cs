namespace ViewLib.Repositories
{
    using System.Threading.Tasks;
    using Data;
    using Services;

    public class CoursesRepository : ICoursesRepository
    {
        private readonly ICache cache;

        public CoursesRepository(ICache cache)
        {
            this.cache = cache;
        }

        public async Task CreateOrUpdateAsync(CourseStamp courses)
        {
            await cache.CreateOrUpdateCoursesAsync(courses);
        }

        public async Task<CourseStamp> ReadAsync()
        {
            var courseStamp = await cache.ReadCoursesAsync();
            return courseStamp ?? new CourseStamp();
        }
    }
}