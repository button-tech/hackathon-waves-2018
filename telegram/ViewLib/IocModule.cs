namespace ViewLib
{
    using DryIoc;
    using BaseNavigation;
    using Data;
    using Helpers;
    using Repositories;
    using Services;

    public static class IocModule
    {
        public static void Load(IRegistrator builder)
        {
            builder.Register<PageNavigator>(Reuse.Singleton);
            builder.Register<PageLocator>(Reuse.Singleton);
            builder.Register<SafeCommandFactory>(Reuse.Singleton);
            builder.Register<ICache, RedisService>(Reuse.Singleton);

            builder.Register<ICoursesRepository, CoursesRepository>(Reuse.Singleton);
            builder.Register<IGuidRepository, GuidRepository>(Reuse.Singleton);
            builder.Register<INavigationRepository, NavigationRepository>(Reuse.Singleton);
            builder.Register<IPropertiesRepository, PropertiesRepository>(Reuse.Singleton);
            
            builder.Register<BasePage>();
            builder.Register<BaseViewModel>();
        }
    }
}
