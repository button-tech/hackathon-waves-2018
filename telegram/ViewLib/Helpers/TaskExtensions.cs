namespace ViewLib.Helpers
{
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static async Task ForgetAsync(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
        }

        public static async void Forget(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
        }
    }
}