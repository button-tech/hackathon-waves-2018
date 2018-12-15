namespace WavesBot.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum CallBack
    {
        Default = 0,
        Back = 1,
        RootPage = 2,
        CreateAccount = 3
    }

    public static class CallBackHelper
    {
        private static readonly IEnumerable<CallBack> CallBacks = Enum.GetValues(typeof(CallBack))
            .Cast<CallBack>();

        public static CallBack Caster(string inp)
        {
            return !string.IsNullOrEmpty(inp) ? CallBacks.SingleOrDefault(x => x.ToString() == inp) : CallBack.Default;
        }

        public static string Name(this CallBack callBack)
        {
            return callBack.ToString();
        }
    }
}