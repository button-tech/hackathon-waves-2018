namespace ViewLib.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ArgumentExceptionExtensions
    {
        /// <summary>
        ///     Выбрасывает исключение <see cref="ArgumentNullException" />, если параметр равен <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Тип параметра.</typeparam>
        /// <param name="argument">Параметр.</param>
        /// <param name="argumentName">Имя параметра.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>(this T argument, string argumentName)
            where T : class
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            return argument;
        }
    }
}