namespace WavesBot.IAL
{
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public class TraceWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(string value)
        {
            Trace.TraceInformation(value);
        }

        public override void Write(object value)
        {
            Trace.TraceInformation(value.ToString());
        }

        public override void Write(string format, object arg0)
        {
            Trace.TraceInformation(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            Trace.TraceInformation(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Trace.TraceInformation(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            Trace.TraceInformation(format, arg);
        }

        public override void WriteLine(string value)
        {
            Trace.TraceInformation(value);
        }

        public override void WriteLine(object value)
        {
            Trace.TraceInformation(value.ToString());
        }

        public override void WriteLine(string format, object arg0)
        {
            Trace.TraceInformation(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            Trace.TraceInformation(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Trace.TraceInformation(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            Trace.TraceInformation(format, arg);
        }
    }
}