namespace ViewLib.BaseNavigation
{
    using DryIoc;
    using System;
    using System.Reflection;
    using Data;
    using Helpers;

    public class PageLocator
    {
        private readonly IResolverContext lifetimeScope;

        public PageLocator(IResolverContext lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public BasePage Resolve<TPage>() where TPage : BasePage
        {
            return lifetimeScope.Resolve<TPage>();
        }

        public BasePage Resolve(Type pageType)
        {
            pageType.ThrowIfNull(nameof(pageType));
            if (!pageType.GetTypeInfo()
                         .IsSubclassOf(typeof(BasePage)))
                throw new ArgumentException($"Argument ({nameof(pageType)}) is not a type of BasePage.");

            var page = (BasePage)lifetimeScope.Resolve(pageType);

            return page;
        }
    }
}