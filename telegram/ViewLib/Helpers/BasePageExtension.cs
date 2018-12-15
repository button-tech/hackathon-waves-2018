namespace ViewLib.Helpers
{
    using Data;

    public static class BasePageExtension
    {
        public static BasePage SetBaseProperties(this BasePage page, long identifier, object context, NavigationType type)
        {
            ((BaseViewModel)page.BindingContent).Identifier = identifier;
            ((BaseViewModel)page.BindingContent).Context = context;
            ((BaseViewModel)page.BindingContent).NavigationType = type;

            return page;
        }
    }
}