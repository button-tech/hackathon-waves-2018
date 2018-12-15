namespace ViewLib.BaseNavigation
{
    using ImTools;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Helpers;
    using Repositories;
    using Services;

    public class PageNavigator
    {
        private readonly ILoggerService loggerService;
        private readonly INavigationRepository navigation;
        private readonly PageLocator pageLocator;
        private readonly IRootPage rootPage;

        public PageNavigator(PageLocator pageLocator,
                             INavigationRepository navigation,
                             ILoggerService loggerService,
                             IRootPage rootPage)
        {
            this.pageLocator = pageLocator;
            this.navigation = navigation;
            this.loggerService = loggerService;
            this.rootPage = rootPage;
        }

        private async Task SafeRefreshViewModelAsync(BaseViewModel baseViewModel)
        {
            try
            {
                await baseViewModel.RefreshPropertiesAsync();
            }
            catch (Exception exception)
            {
                loggerService.LogAsync($"{exception.Message}\n{exception.StackTrace}")
                             .Forget();
            }
        }

        public async Task PushPageAsync(Type pageType, long identifier, object context, NavigationType type = NavigationType.Default)
        {
            var container = await navigation.ReadAsync(identifier);
            var page = pageLocator.Resolve(pageType);

            container = container.Append(page.GetType());

            await navigation.CreateOrUpdateAsync(identifier, container);
            await RenderPage(identifier, context, page, type);
        }

        public async Task PushPageAsync<TPage>(long identifier, object context, NavigationType type = NavigationType.Default)
            where TPage : BasePage
        {
            var container = await navigation.ReadAsync(identifier);
            var page = pageLocator.Resolve(typeof(TPage));

            container = container.Append(page.GetType());

            await navigation.CreateOrUpdateAsync(identifier, container);
            await RenderPage(identifier, context, page, type);
        }

        public async Task TryPopPageAsync(long identifier, object context, NavigationType type = NavigationType.Default)
        {
            var container = await navigation.ReadAsync(identifier);

            BasePage page;
            if (!container.Any() ||
                container.Length == 1)
            {
                page = pageLocator.Resolve(rootPage.RootPageType());
            }
            else
            {
                container = container.RemoveAt(container.Length - 1);
                page = pageLocator.Resolve(container[container.Length - 1]);
            }

            await navigation.CreateOrUpdateAsync(identifier, container);
            await RenderPage(identifier, context, page, type);
        }

        public async Task PopToRootAsync(long identifier, object context, bool withRender = true, NavigationType type = NavigationType.Default)
        {
            var container = new Type[0];
            var page = pageLocator.Resolve(rootPage.RootPageType());

            await navigation.CreateOrUpdateAsync(identifier, container);
            await RenderPage(identifier, context, page, type);
        }

        public async Task RemoveAllNavigationStack(long identifier)
        {
            await navigation.DeleteAsync(identifier);
        }

        public async Task RestoreLastPage(long identifier, object context, bool force = false, NavigationType type = NavigationType.RestoreState)
        {
            var container = await navigation.ReadAsync(identifier);

            BasePage page;
            if (!container.Any())
            {
                page = pageLocator.Resolve(rootPage.RootPageType());
                container = container.Append(page.GetType());
            }
            else
            {
                var lastPageType = container[container.Length - 1];
                page = pageLocator.Resolve(lastPageType);
            }

            await navigation.CreateOrUpdateAsync(identifier, container);
            await RenderPage(identifier, context, page, type);
        }

        private async Task RenderPage(long identifier, object context, BasePage page, NavigationType type, bool withRender = true)
        {
            page.SetBaseProperties(identifier, context, type);

            await SafeRefreshViewModelAsync((BaseViewModel)page.BindingContent);

            if (withRender)
                await page.RenderPage();
        }
    }
}