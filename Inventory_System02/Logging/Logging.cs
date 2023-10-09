using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_System02.Logging
{
    public class Logger
    {
        private static readonly object lockObj = new object();
        private static Logger instance;

        public Logger()
        {
            // Private constructor to prevent direct instantiation.
        }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new Logger();
                        }
                    }
                }
                return instance;
            }
        }

        public void LogError(string message)
        {
            Log("ERROR", message);
        }

        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        private void Log(string level, string message)
        {
            // Use the System.Diagnostics namespace to write to the event log.
            EventLog eventLog = new EventLog("Application");
            eventLog.Source = "InventoryMS";

            eventLog.WriteEntry($"[{level}] {message}", EventLogEntryType.Information);
        }
    }

}
