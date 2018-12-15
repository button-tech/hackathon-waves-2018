namespace WavesBot.UI.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using IAL;
    using Services;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using ViewLib.BaseNavigation;
    using ViewLib.Data;
    using ViewLib.Helpers;
    using ViewLib.Repositories;
    using ViewLib.Services;

    public abstract class BaseTelegramViewModel : BaseViewModel
    {
        public BaseTelegramViewModel(PageNavigator pageNavigator, SafeCommandFactory commandFactory,
            Func<DateTimeOffset> timeStamp, ILoggerService loggerService,
            IPropertiesRepository propertiesRepository,
            BotService botService)
            : base(pageNavigator, commandFactory, timeStamp, loggerService, propertiesRepository)
        {
            BotService = botService;

            SendMessage = CommandFactory.Create<string>(SendMessageAsync);
            SendInlineKeyboard = CommandFactory.Create<InlineMenu>(SendInlineKeyboardAsync);
            SendReplyKeyboard = CommandFactory.Create<ReplyMenu>(SendKeyboardAsync);
            GoBack = CommandFactory.Create(GoBackAsync);
        }

        /// <summary>
        /// Возвращает CallBack[], по которым можно отрендерить данную страницу.
        /// </summary>
        public virtual Task<CallBack[]> GetPageCallBacks() => Task.FromResult(new CallBack[0]);

        /// <summary>
        /// Приведенный к int CallBack, по которому может рендерится страница.
        /// </summary>
        public int Caster => (int) CallBackHelper.Caster(DialogContext?.CallbackQuery?.Data);

        public bool IsCallBack => DialogContext.Type == UpdateType.CallbackQuery;

        public override async Task RefreshPropertiesAsync()
        {
            await base.RefreshPropertiesAsync();

            if (!IsCallBack) return;

            if (DialogContext?.CallbackQuery.Data != CallBack.Back.ToString() &&
                DialogContext?.CallbackQuery.Data != CallBack.RootPage.ToString())
                await CallBackAction();
        }

        /// <summary>
        /// Метод выполняется, если рендер вызван по CallBack.
        /// Здесь определяются операции, необходимые для рендера страницы по CallBack
        /// </summary>
        /// <returns></returns>.
        protected virtual Task CallBackAction()
        {
            return Task.CompletedTask;
        }

        protected BotService BotService { get; }

        protected override Func<DateTimeOffset> TimeStamp
        {
            get
            {
                if (DialogContext == null)
                    return base.TimeStamp;

                DateTimeOffset timeStamp = DialogContext?.Message?.Date ?? DialogContext.CallbackQuery.Message.Date;
                return () => timeStamp;
            }
        }

        public bool? IsDefaultRenderNeed
        {
            get => GetPersistentValue<bool?>() ?? true;
            set => SetPersistentValue(value, onChanged: SaveChanges);
        }

        public int UserId => DialogContext?.CallbackQuery?.From?.Id ??
                             DialogContext?.Message?.From?.Id ??
                             //todo:localize
                             throw new Exception("UserId not exist");

        public string NickName => DialogContext?.CallbackQuery?.From?.Username ??
                                  DialogContext?.Message?.From?.Username ??
                                  //todo:localize
                                  string.Empty;

        public int MessageId => DialogContext?.Message?.MessageId ??
                                DialogContext?.CallbackQuery?.Message?.MessageId ??
                                //todo:localize
                                throw new Exception("MessageId not exist");

        public string MessageText => DialogContext?.Message?.Text ?? DialogContext?.CallbackQuery?.Message.Text;

        public Update DialogContext => Context as Update;

        public Safe SendMessage { get; }
        //public Safe SendMessage => CommandFactory.Create<string>(SendMessageAsync);

        public Safe SendReplyKeyboard { get; }

        public Safe SendInlineKeyboard { get; }

        private async Task SendMessageAsync(string arg)
        {
            await BotService.Client.SendTextMessageAsync(Identifier, arg, ParseMode.Markdown);
        }

        private async Task SendKeyboardAsync(ReplyMenu replyMenu)
        {
            replyMenu.Keyboard.ResizeKeyboard = true;
            await BotService.Client.SendTextMessageAsync(Identifier, replyMenu.Text, ParseMode.Markdown,
                replyMarkup: replyMenu.Keyboard);
        }

        private async Task SendInlineKeyboardAsync(InlineMenu inlineMenu)
        {
            //Это нормальный кейс. Телеграм бросает ошибку, если попробовать
            //перезаписать сообщение, защищенное от записи.
            //В таком случае мы просто игнорируем это и отправляем новое сообщение, попробовав удалив предыдущее.
            try
            {
                await BotService.Client.EditMessageTextAsync(Identifier, MessageId,
                    inlineMenu.Text,
                    ParseMode.Markdown,
                    replyMarkup: inlineMenu.Keyboard);
            }
            catch (Exception)
            {
                try
                {
                    await BotService.Client.DeleteMessageAsync(Identifier, MessageId);
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    await BotService.Client.SendTextMessageAsync(Identifier, inlineMenu.Text, ParseMode.Markdown,
                        replyMarkup: inlineMenu.Keyboard);
                }
            }
        }

        public Safe GoBack { get; }

        private async Task GoBackAsync()
        {
            await PageNavigator.TryPopPageAsync(Identifier, Context);
        }
    }
}