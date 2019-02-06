using System;
using System.Globalization;

namespace LoggingLibrary
{
    public class LogMessage : ILogMessage
    {

        public object[] Args { get; private set; }

        public IFormatProvider Provider { get; private set; }

        public string Format { get; private set; }

        public LogMessage(string message)
          : this((IFormatProvider)CultureInfo.InvariantCulture, message, new object[0])
        {
        }

        public LogMessage(string format, params object[] args)
          : this((IFormatProvider)CultureInfo.InvariantCulture, format, args)
        {
        }

        public LogMessage(IFormatProvider provider, string format, params object[] args)
        {
            Args = args;
            Format = format;
            Provider = provider;
        }

        public override string ToString()
        {
            return string.Format(Provider, Format, Args);
        }
    }
}
