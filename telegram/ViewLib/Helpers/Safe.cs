namespace ViewLib.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading.Tasks;

    public class Safe
    {
        private readonly Func<object, bool> canExecute;
        private readonly Func<object, Task> execute;
        private readonly Func<Exception, Task> handler;

        public Safe(Func<Task> execute, Func<Exception, Task> handler = null)
            : this(o => execute(), handler)
        {
        }

        public Safe(Func<object, Task> execute, Func<Exception, Task> handler = null)
        {
            this.execute = execute;
            this.handler = handler;
        }

        public Safe(Func<Task> execute, Func<bool> canExecute,
                    Func<Exception, Task> handler = null)
        {
            this.execute = o => execute();
            this.canExecute = o => canExecute();
            this.handler = handler;
        }

        public Safe(Func<object, Task> execute, Func<object, bool> canExecute,
                    Func<Exception, Task> handler = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.handler = handler;
        }

        public bool CanExecute(object parameter)
        {
            return parameter == null || canExecute == null || canExecute(parameter);
        }

        public async void Execute(object parameter = null)
        {
            try
            {
                await execute(parameter);
            }
            catch (Exception exception)
            {
                await handler(exception);
                Debug.WriteLine(exception.Message);
            }
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void ChangeCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class Safe<T> : Safe
    {
        public Safe(Func<T, Task> execute, Func<Exception, Task> handler = null)
            : base(o => IsValidParameter(o) ? execute((T)o) : Task.CompletedTask, handler)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
        }

        public Safe(Func<T, Task> execute, Func<T, bool> canExecute,
                    Func<Exception, Task> handler = null) : base(
            o => IsValidParameter(o) ? execute((T)o) : Task.CompletedTask,
            o => IsValidParameter(o) && canExecute((T)o),
            handler)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));
        }

        private static bool IsValidParameter(object o)
        {
            if (o != null) return o is T;

            var type = typeof(T);

            if (Nullable.GetUnderlyingType(type) != null) return true;

            return !type.GetTypeInfo()
                        .IsValueType;
        }
    }
}