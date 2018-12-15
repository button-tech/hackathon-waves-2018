namespace WavesBot.Services
{
    using System;
    using System.Threading.Tasks;
    using ViewLib.Services;

    public class LoggerService : ILoggerService
    {
        public Task LogAsync(string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }

        public Task LogExceptionAsync(Exception exception)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }
    }
}