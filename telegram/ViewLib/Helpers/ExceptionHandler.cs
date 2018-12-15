namespace ViewLib.Helpers
{
    using System;
    using System.Threading.Tasks;
    using Services;

    public class ExceptionHandler
    {
        private readonly ILoggerService loggerService;

        public ExceptionHandler(ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }

        public Task HanldeAsync(Exception exception)
        {
            loggerService.LogAsync($"{exception.Message}\n{exception.StackTrace}")
                         .Forget();

            return Task.CompletedTask;
        }
    }
}