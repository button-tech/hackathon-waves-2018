namespace ViewLib.Services
{
    using System;
    using System.Threading.Tasks;

    //todo: Дописать когда-нибудь логгер
    public interface ILoggerService
    {
        Task LogAsync(string message);

        Task LogExceptionAsync(Exception exception);
    }
}