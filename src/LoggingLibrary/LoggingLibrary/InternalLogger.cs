using System;

namespace LoggingLibrary
{
    internal abstract class InternalLogger
    {
        public Type DeclaringType { get; private set; }

        public bool IsEnabled { get; set; }

        public InternalLogger(Type declaringType)
        {
            DeclaringType = declaringType;
            IsEnabled = true;
        }

        public virtual bool IsErrorEnabled
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsDebugEnabled
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsInfoEnabled
        {
            get
            {
                return true;
            }
        }

        public abstract void Error(Exception exception, string messageFormat, params object[] args);

        public abstract void Debug(Exception exception, string messageFormat, params object[] args);

        public abstract void DebugFormat(string message, params object[] arguments);

        public abstract void InfoFormat(string message, params object[] arguments);
    }
}
