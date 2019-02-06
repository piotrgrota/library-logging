using System;

namespace LoggingLibrary
{
    public interface ILogMessage
    {

        string Format { get; }


        object[] Args { get; }


        IFormatProvider Provider { get; }
    }
}
