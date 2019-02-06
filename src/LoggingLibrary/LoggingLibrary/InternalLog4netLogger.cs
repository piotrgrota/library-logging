using System;
using System.Globalization;
using System.Reflection;

namespace LoggingLibrary
{
    internal class InternalLog4netLogger : InternalLogger
    {
        private enum LoadState
        {
            Uninitialized,
            Failed,
            Loading,
            Success,
        }

        private static LoadState loadState = LoadState.Uninitialized;
        private static readonly object _lock = new object();
        private static Type logMangerType;
        private static MethodInfo getLoggerWithTypeMethod;
        private static Type logType;
        private static MethodInfo logMethod;
        private static Type levelType;
        private static object debugLevelPropertyValue;
        private static object infoLevelPropertyValue;
        private static object errorLevelPropertyValue;
        private static MethodInfo isEnabledForMethod;
        private static Type systemStringFormatType;
        private static Type loggerType;
        private object internalLogger;
        private bool? isErrorEnabled;
        private bool? isDebugEnabled;
        private bool? isInfoEnabled;


        private static void loadStatics()
        {
            lock (_lock)
            {
                if (loadState != LoadState.Uninitialized)
                {
                    return;
                }
                loadState = LoadState.Loading;
                try
                {
                    loggerType = typeof(Logger);
                    logMangerType = Type.GetType("log4net.Core.LoggerManager, log4net");

                    if (logMangerType == null)
                    {
                        loadState = LoadState.Failed;
                    }
                    else
                    {
                        getLoggerWithTypeMethod = logMangerType.GetMethod("GetLogger", new Type[] { typeof(Assembly), typeof(Type) });
                        logType = Type.GetType("log4net.Core.ILogger, log4net");

                        levelType = Type.GetType("log4net.Core.Level, log4net");

                        debugLevelPropertyValue = levelType.GetField("Debug").GetValue(null);
                        infoLevelPropertyValue = levelType.GetField("Info").GetValue(null);
                        errorLevelPropertyValue = levelType.GetField("Error").GetValue(null);
                        systemStringFormatType = Type.GetType("log4net.Util.SystemStringFormat, log4net");
                        logMethod = logType.GetMethod("Log", new Type[] { typeof(Type), levelType, typeof(object), typeof(Exception) });
                        isEnabledForMethod = logType.GetMethod("IsEnabledFor", new Type[] { levelType });

                        if (getLoggerWithTypeMethod == null || isEnabledForMethod == null || (logType == null || levelType == null) || logMethod == null)
                        {
                            loadState = LoadState.Failed;
                        }
                        else
                        {

                            var typeInfo = Type.GetType("log4net.Config.XmlConfigurator, log4net");
                            if (typeInfo != null)
                            {
                                MethodInfo method = typeInfo.GetMethod("Configure", new Type[0]);
                                if (method != null)
                                {
                                    method.Invoke(null, null);
                                }

                            }
                            loadState = LoadState.Success;
                        }
                    }
                }
                catch
                {
                    loadState = LoadState.Failed;
                }
            }
        }

        public InternalLog4netLogger(Type declaringType) : base(declaringType)
        {
            if (loadState == LoadState.Uninitialized)
            {
                loadStatics();
            }

            if (logMangerType == null)
            {
                return;
            }

            internalLogger = getLoggerWithTypeMethod.Invoke(null, new object[2] { declaringType.Assembly, declaringType });
        }


        public override bool IsErrorEnabled
        {
            get
            {
                if (!isErrorEnabled.HasValue)
                {
                    if (IsNotInitialized() || errorLevelPropertyValue == null)
                    {
                        isErrorEnabled = new bool?(false);
                    }
                    else
                    {
                        isErrorEnabled = new bool?(Convert.ToBoolean(isEnabledForMethod.Invoke(internalLogger, new object[1]
                        {
                            errorLevelPropertyValue
                        }), (IFormatProvider)CultureInfo.InvariantCulture));
                    }
                }
                return isErrorEnabled.Value;
            }
        }

        public override void Error(Exception exception, string messageFormat, params object[] args)
        {
            logMethod.Invoke(internalLogger, new object[4]
            {
                loggerType,
                errorLevelPropertyValue,
                new LogMessage((IFormatProvider) CultureInfo.InvariantCulture, messageFormat, args),
                exception
            });
        }

        private bool IsNotInitialized()
        {
            return loadState != LoadState.Success || internalLogger == null || (loggerType == null || systemStringFormatType == null);
        }

        public override bool IsDebugEnabled
        {
            get
            {
                if (!isDebugEnabled.HasValue)
                {
                    if (IsNotInitialized() || debugLevelPropertyValue == null)
                    {
                        isDebugEnabled = new bool?(false);
                    }
                    else
                    {
                        isDebugEnabled = new bool?(Convert.ToBoolean(isEnabledForMethod.Invoke(internalLogger, new object[1]
                        {
                            debugLevelPropertyValue
                        }), (IFormatProvider)CultureInfo.InvariantCulture));
                    }
                }
                return isDebugEnabled.Value;
            }
        }
        public override void Debug(Exception exception, string messageFormat, params object[] args)
        {
            logMethod.Invoke(internalLogger, new object[]
            {
                loggerType,
                debugLevelPropertyValue,
                 new LogMessage((IFormatProvider) CultureInfo.InvariantCulture, messageFormat, args),
                 exception
            });
        }


        public override void DebugFormat(string message, params object[] arguments)
        {
            logMethod.Invoke(internalLogger, new object[4]
            {
                loggerType,
                debugLevelPropertyValue,
                new LogMessage((IFormatProvider) CultureInfo.InvariantCulture, message, arguments),
                null
            });
        }



        public override bool IsInfoEnabled
        {
            get
            {
                if (!isInfoEnabled.HasValue)
                {
                    if (IsNotInitialized() || infoLevelPropertyValue == null)
                    {
                        isInfoEnabled = new bool?(false);
                    }

                    else
                    {
                        isInfoEnabled = new bool?(Convert.ToBoolean(isEnabledForMethod.Invoke(this.internalLogger, new object[1]
                        {
                            infoLevelPropertyValue
                        }), (IFormatProvider)CultureInfo.InvariantCulture));
                    }

                }
                return isInfoEnabled.Value;
            }
        }

        public override void InfoFormat(string message, params object[] arguments)
        {
            logMethod.Invoke(this.internalLogger, new object[4]
             {
                loggerType,
                infoLevelPropertyValue,
                new LogMessage((IFormatProvider) CultureInfo.InvariantCulture, message, arguments),null
             });
        }
    }
}
