using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingLibrary
{
    public class TestLogger
    {
        public static void TestLogs()
        {
            var t = Logger.GetLogger<TestLogger>();
            t.InfoFormat("Logs from Library");
        }
    }
}
