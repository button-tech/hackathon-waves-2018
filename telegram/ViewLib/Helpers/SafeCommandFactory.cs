namespace ViewLib.Helpers
{
    using System;
    using System.Threading.Tasks;
    using Services;

    public class SafeCommandFactory
    {
        public SafeCommandFactory(ILoggerService loggerService)
        {
            DefaultExceptionHandler = new ExceptionHandler(loggerService);
        }

        protected ExceptionHandler DefaultExceptionHandler { get; }

        public Safe Create(Func<Task> execute, Func<Exception, Task> exceptionHandler = null)
        {
            return new Safe(execute, exceptionHandler ?? DefaultExceptionHandler.HanldeAsync);
        }

        public Safe Create(Func<object, Task> execute, Func<Exception, Task> exceptionHandler = null)
        {
            return new Safe(execute, exceptionHandler ?? DefaultExceptionHandler.HanldeAsync);
        }

        public Safe Create(Func<Task> execute, Func<bool> canExecute,
                           Func<Exception, Task> exceptionHandler = null)
        {
            return new Safe(execute, canExecute, exceptionHandler ?? DefaultExceptionHandler.HanldeAsync);
        }

        public Safe<TResult> Create<TResult>(Func<TResult, Task> execute,
                                             Func<Exception, Task> exceptionHandler = null)
        {
            return new Safe<TResult>(execute, exceptionHandler ?? DefaultExceptionHandler.HanldeAsync);
        }

        public Safe<T> Create<T>(Func<T, Task> execute, Func<T, bool> canExecute,
                                 Func<Exception, Task> exceptionHandler = null)
        {
            return new Safe<T>(execute, canExecute, exceptionHandler ?? DefaultExceptionHandler.HanldeAsync);
        }
    }
}