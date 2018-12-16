namespace WavesBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using DryIoc;
    using IAL;
    using Services;
    using UI.Pages;
    using UI.ViewModels;
    using ViewLib.BaseNavigation;
    using ViewLib.Services;
    using LoggerService = Services.LoggerService;

    public class IocModule
    {
        public static void Load(IRegistrator builder)
        {
            builder.Register<BotService>(Reuse.Singleton);
            builder.Register<AccountService>(Reuse.Singleton);
            builder.Register<GuidService>(Reuse.Singleton);
            builder.Register<StateSynchronizer>(Reuse.Singleton);
            builder.Register<UserProvider>(Reuse.Singleton);
            
            builder.Register<IRootPage, TelegramRootPage>(Reuse.Singleton);
            builder.Register<ILoggerService, LoggerService>(Reuse.Singleton);
            builder.Register<IUserDataRepository, UserDataRepository>(Reuse.Singleton);

            builder.RegisterMany(GetAssemblyPageTypes());
            builder.RegisterMany(GetAssemblyViewModelsTypes());
        }

        private static IEnumerable<Type> GetAssemblyPageTypes()
        {
            return typeof(BaseTelegramPage).GetTypeInfo()
                .Assembly.DefinedTypes
                .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract && typeInfo.Name.Contains(@"Page")
                                   && typeInfo.BaseType == typeof(BaseTelegramPage))
                .Select(x => x.AsType());
        }

        private static IEnumerable<Type> GetAssemblyViewModelsTypes()
        {
            return typeof(BaseTelegramViewModel).GetTypeInfo()
                .Assembly.DefinedTypes
                .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract && typeInfo.Name.Contains(@"ViewModel")
                                   && typeInfo.BaseType == typeof(BaseTelegramViewModel))
                .Select(x => x.AsType());
        }
    }
}