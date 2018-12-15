namespace WavesBot.Services
{
    using Telegram.Bot;

    public class BotService
    {
        public BotService(BotConfiguration config)
        {
            var botConfiguration = config;

            Client = new TelegramBotClient(botConfiguration.BotToken);

            LoggerClient = new TelegramBotClient(botConfiguration.LoggerBotToken);
        }

        public TelegramBotClient Client { get; }

        public TelegramBotClient LoggerClient { get; }
    }
}