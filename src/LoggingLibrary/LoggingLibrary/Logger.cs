using System;
using System.Collections.Generic;

namespace LoggingLibrary
{
    public class Logger : ILogger
    {
        private static IDictionary<Type, Logger> initializedLoogers = new Dictionary<Type, Logger>();

        private InternalLogger logger;

        private Logger(Type type)
        {
            logger = new InternalLog4netLogger(type);
            logger.IsEnabled = true;
        }

        public static Logger GetLogger(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Logger logger;
            lock (initializedLoogers)
            {
                if (!initializedLoogers.TryGetValue(type, out logger))
                {
                    logger = new Logger(type);
                    initializedLoogers[type] = logger;
                }
            }
            return logger;
        }

        public static Logger GetLogger<T>() where T : class
        {
            return GetLogger(typeof(T));
        }

        public static void ClearLoggerCache()
        {
            lock (initializedLoogers)
            {
                initializedLoogers = new Dictionary<Type, Logger>();
            }
        }

        public void Error(Exception exception, string messageFormat, params object[] args)
        {
            if (logger.IsEnabled && logger.IsErrorEnabled)
            {
                logger.Error(exception, messageFormat, args);
            }
        }

        public void Debug(Exception exception, string messageFormat, params object[] args)
        {
            if (logger.IsEnabled && logger.IsDebugEnabled)
            {
                logger.Debug(exception, messageFormat, args);
            }
        }

        public void DebugFormat(string messageFormat, params object[] args)
        {
            if (logger.IsEnabled && logger.IsDebugEnabled)
            {
                logger.DebugFormat(messageFormat, args);
            }
        }

        public void InfoFormat(string messageFormat, params object[] args)
        {
            if (logger.IsEnabled && logger.IsInfoEnabled)
            {
                logger.InfoFormat(messageFormat, args);
            }
        }
    }
}
