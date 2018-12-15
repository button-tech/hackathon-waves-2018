namespace ViewLib.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Services;

    public class PropertiesRepository : IPropertiesRepository
    {
        private readonly ICache cache;

        public PropertiesRepository(ICache cache)
        {
            this.cache = cache;
        }

        public async Task CreateOrUpdateAsync(long identifier, Dictionary<string, object> propertiesDictionary)
        {
            var stamp = new PropertiesStamp
            {
                Value = propertiesDictionary
            };

            await cache.CreateOrUpdatePropertiesAsync(identifier, stamp);
        }

        public async Task<Dictionary<string, object>> ReadAsync(long identifier)
        {
            var properties = await cache.ReadPropertiesAsync(identifier);
            return properties?.Value ?? new Dictionary<string, object>();
        }

        public async Task DeleteAsync(long identifier)
        {
            await cache.DeletePropertiesAsync(identifier);
        }
    }
}