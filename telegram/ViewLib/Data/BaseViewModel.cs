namespace ViewLib.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Utf8Json;
    using BaseNavigation;
    using Helpers;
    using Repositories;
    using Services;

    public class BaseViewModel
    {
        private readonly ILoggerService loggerService;
        private readonly IPropertiesRepository propertiesRepository;
        protected Dictionary<string, object> Properties;

        public BaseViewModel(PageNavigator pageNavigator,
                             SafeCommandFactory commandFactory,
                             Func<DateTimeOffset> timeStamp,
                             ILoggerService loggerService,
                             IPropertiesRepository propertiesRepository)
        {
            this.loggerService = loggerService;
            this.propertiesRepository = propertiesRepository;
            TimeStamp = timeStamp;
            PageNavigator = pageNavigator;
            CommandFactory = commandFactory;
        }

        public NavigationType NavigationType { get; set; }

        public object Context { protected get; set; }

        protected virtual Func<DateTimeOffset> TimeStamp { get; }

        public long Identifier { protected get; set; }

        protected PageNavigator PageNavigator { get; }

        protected SafeCommandFactory CommandFactory { get; }

        public virtual async Task RefreshPropertiesAsync()
        {
            Properties = await propertiesRepository.ReadAsync(Identifier);
        }

        public async void SaveChanges()
        {
            try
            {
                await propertiesRepository.CreateOrUpdateAsync(Identifier, Properties);
            }
            catch (Exception ex)
            {
                await loggerService.LogAsync(ex.Message);
            }
        }

        public async Task SaveChangesAsync()
        {
            await propertiesRepository.CreateOrUpdateAsync(Identifier, Properties);
        }

        protected T GetPersistentValue<T>([CallerMemberName] string propertyName = "")
        {
            var key = GeneratePropertyKey(GetType(), propertyName);

            return GetPersistentValueOrDefault<T>(key);
        }

        protected void SetPersistentValue<T>(T value, [CallerMemberName] string propertyName = "",
                                             Action onChanged = null)
        {
            var key = GeneratePropertyKey(GetType(), propertyName);
            onChanged?.Invoke();
            Properties[key] = JsonSerializer.ToJsonString(value);
        }

        public void SetPersistentValueFor<TViewModel>(object value, string propertyName)
            where TViewModel : BaseViewModel
        {
            var key = GeneratePropertyKey(typeof(TViewModel), propertyName);

            Properties[key] = JsonSerializer.ToJsonString(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GeneratePropertyKey(Type ownerType, string propertyName)
        {
            return $"{ownerType.FullName}.{propertyName}";
        }

        protected T GetPersistentValueOrDefault<T>(string key)
        {
            var hasValue = Properties.TryGetValue(key, out var property);
            if (!hasValue)
                return default(T);

            try
            {
                return JsonSerializer.Deserialize<T>((string)property);
            }
            catch (Exception exception)
            {
                loggerService.LogAsync($"{exception.Message}\n{exception.StackTrace}")
                             .Forget();
            }

            return default(T);
        }
    }
}