namespace WavesBot.IAL
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Services;
    using ViewLib.Services;

    public class LoggerService : ILoggerService
    {
        private readonly Func<DateTimeOffset> now;
        private readonly BotService botService;

        public LoggerService(Func<DateTimeOffset> now, BotService botService)
        {
            this.now = now;
            this.botService = botService;
        }

        public async Task LogAsync(string message)
        {
            var time = now().AddHours(3);
            var log = new Log
            {
                TimeStamp = time,
                LogFromType = LogFromTypes.TelegramBot,
                Message = message,
                Type = LogTypes.Warning
            };
            await WriteAsync(log);
        }

        public async Task LogExceptionAsync(Exception exception)
        {
            var time = now().AddHours(3);
            var log = new Log
            {
                TimeStamp = time,
                LogFromType = LogFromTypes.TelegramBot,
                Message = $"{exception.Message}\n" +
                          $"{exception.StackTrace}\n" +
                          $"{time}",
                Type = LogTypes.Exception
            };
            await WriteAsync(log);
        }

        public async Task LogFrontAsync(Log log)
        {
            log.TimeStamp = now().AddHours(3);
            await LogToBotAsync(log);
        }

        private async Task WriteAsync(Log log)
        {
            await LogToBotAsync(log);
        }

        private async Task LogToBotAsync(Log log)
        {
            foreach (var id in ViewConstants.DeveloperIds)
            {
                try
                {
                    var text =
                        $"{log.LogFromType.ToString()} {log.Type.ToString()} ({log.TimeStamp:MM/dd/yy HH:mm})\n" +
                        $"{log.Message}";
                    await botService.LoggerClient.SendTextMessageAsync(id, text);
                }
                catch (Exception)
                {
                    //ignore
                }
            }
        }
    }
}