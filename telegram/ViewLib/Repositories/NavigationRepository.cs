namespace ViewLib.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using Services;

    public class NavigationRepository : INavigationRepository
    {
        private readonly ICache cache;

        public NavigationRepository(ICache cache)
        {
            this.cache = cache;
        }

        public async Task CreateOrUpdateAsync(long identifier, Type[] navigationArray)
        {
            var stamp = new NavigationStamp
            {
                Value = navigationArray
            };

            await cache.CreateOrUpdateNavigationAsync(identifier, stamp);
        }

        public async Task<Type[]> ReadAsync(long identifier)
        {
            var navigationStamp = await cache.ReadNavigationAsync(identifier);
            return navigationStamp?.Value ?? new Type[0];
        }

        public async Task DeleteAsync(long identifier)
        {
            await cache.DeleteNavigationAsync(identifier);
        }
    }
}