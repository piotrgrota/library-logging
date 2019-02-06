using log4net;
using LoggingLibrary;

namespace LoggingLibraryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestLogger.TestLogs();
            var log4Netlogger = LogManager.GetLogger(typeof(Program));
            log4Netlogger.Info("Log4Net Logger");
        }
    }
}
