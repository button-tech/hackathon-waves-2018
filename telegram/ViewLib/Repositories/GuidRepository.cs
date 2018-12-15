namespace ViewLib.Repositories
{
    using System.Threading.Tasks;
    using Data;
    using Services;

    public class GuidRepository : IGuidRepository
    {
        private readonly ICache cache;

        public GuidRepository(ICache cache)
        {
            this.cache = cache;
        }

        public async Task<TelegramGuidStamp> ReadGuidAsync(string guid)
        {
            return await cache.ReadGuidAsync(guid);
        }

        public async Task DeleteGuidAsync(string guid)
        {
            await cache.DeleteGuidAsync(guid);
        }

        public async Task CreateOrUpdateGuidAsync(string guid, TelegramGuidStamp guidStamp)
        {
            await cache.CreateOrUpdateGuidAsync(guid, guidStamp);
        }
    }
}