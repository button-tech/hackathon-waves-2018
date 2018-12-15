namespace ViewLib.Services
{
    using StackExchange.Redis;
    using System;
    using System.Threading.Tasks;
    using Utf8Json;
    using Data;

    public class RedisService : ICache
    {
        private static string devKey;
        private readonly Lazy<ConnectionMultiplexer> lazyConnection;

        public RedisService(Func<ConnectionMultiplexer> connectionMultiplexerFactory)
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(connectionMultiplexerFactory);

            devKey = new Guid().ToString()
                               .Substring(0, 5);

#if RELEASE
            devKey = string.Empty;
#endif
        }

        private ConnectionMultiplexer Connection => lazyConnection.Value;

        #region Keys

        private static string TelegramPropKey(long identifier)
        {
            return $"{devKey}telegramPropKey:{identifier}";
        }

        private static string TelegramNavigationKey(long identifier)
        {
            return $"{devKey}telegramNavigationKey:{identifier}";
        }

        private static string TelegramGuidKey(string identifier)
        {
            return $"{devKey}telegramGuidKey:{identifier}";
        }

        private static string TelegramCourses()
        {
            return $"{devKey}telegramCoursesKey";
        }

        #endregion

        #region Guid

        public async Task<TelegramGuidStamp> ReadGuidAsync(string guid)
        {
            var database = Connection.GetDatabase();
            var key = TelegramGuidKey(guid);
            var value = await database.StringGetAsync(key);

            return value == RedisValue.Null
                ? default(TelegramGuidStamp)
                : JsonSerializer.Deserialize<TelegramGuidStamp>((byte[])value);
        }

        public async Task DeleteGuidAsync(string guid)
        {
            var database = Connection.GetDatabase();
            var key = TelegramGuidKey(guid);
            await database.KeyDeleteAsync(key);
        }

        public async Task CreateOrUpdateGuidAsync(string guid, TelegramGuidStamp guidStamp)
        {
            var database = Connection.GetDatabase();
            var key = TelegramGuidKey(guid);
            guidStamp.LifetimeToDelete = DateTimeOffset.Now.AddMinutes(10)
                                                       .UtcDateTime;

            var value = JsonSerializer.Serialize(guidStamp);

            await database.StringSetAsync(key, value, TimeSpan.FromMinutes(10));
        }

        #endregion

        #region Properties 

        public async Task<PropertiesStamp> ReadPropertiesAsync(long identifier)
        {
            var database = Connection.GetDatabase();
            var key = TelegramPropKey(identifier);
            var value = await database.StringGetAsync(key);

            return value == RedisValue.Null
                ? default(PropertiesStamp)
                : JsonSerializer.Deserialize<PropertiesStamp>((byte[])value);
        }

        public async Task CreateOrUpdatePropertiesAsync(long identifier, PropertiesStamp props)
        {
            var database = Connection.GetDatabase();
            var key = TelegramPropKey(identifier);
            var value = JsonSerializer.Serialize(props);

            await database.StringSetAsync(key, value, TimeSpan.FromMinutes(15));
        }

        public async Task DeletePropertiesAsync(long identifier)
        {
            var database = Connection.GetDatabase();
            var key = TelegramPropKey(identifier);
            await database.KeyDeleteAsync(key);
        }

        #endregion

        #region Navigation

        public async Task<NavigationStamp> ReadNavigationAsync(long identifier)
        {
            var database = Connection.GetDatabase();
            var key = TelegramNavigationKey(identifier);
            var value = await database.StringGetAsync(key);

            var result = value == RedisValue.Null
                ? default(NavigationStamp)
                : JsonSerializer.Deserialize<NavigationStamp>((byte[])value);

            return result;
        }

        public async Task CreateOrUpdateNavigationAsync(long identifier, NavigationStamp navigationCache)
        {
            var database = Connection.GetDatabase();
            var key = TelegramNavigationKey(identifier);
            var value = JsonSerializer.Serialize(navigationCache);

            await database.StringSetAsync(key, value, TimeSpan.FromMinutes(15));
        }

        public async Task DeleteNavigationAsync(long identifier)
        {
            var database = Connection.GetDatabase();
            var key = TelegramNavigationKey(identifier);
            await database.KeyDeleteAsync(key);
        }

        #endregion

        #region Cources

        public async Task<CourseStamp> ReadCoursesAsync()
        {
            var database = Connection.GetDatabase();
            var key = TelegramCourses();
            var value = await database.StringGetAsync(key);

            var result = value == RedisValue.Null
                ? default(CourseStamp)
                : JsonSerializer.Deserialize<CourseStamp>((byte[])value);

            return result;
        }

        public async Task CreateOrUpdateCoursesAsync(CourseStamp courseStamp)
        {
            var database = Connection.GetDatabase();
            var key = TelegramCourses();
            var value = JsonSerializer.Serialize(courseStamp);

            await database.StringSetAsync(key, value, TimeSpan.FromMinutes(6));
        }

        #endregion
    }
}