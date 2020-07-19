using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public static class Debug
    {

        private static List<Logger> Loggers = new List<Logger>(1);

        public static void AddLogger(Logger logger)
        {
            if (logger == null) return;

            Loggers.Add(logger);
        }

        public static void RemoveLogger(Logger logger)
        {
            if (logger == null) return;

            Loggers.Remove(logger);
        }

        public static void Log(object message)
        {
            foreach (Logger logger in Loggers)
            {
                logger.Log(message ?? "Null");
            }
        }
        public static void LogWarning(object message)
        {
            foreach (Logger logger in Loggers)
            {
                logger.LogWarning(message ?? "Null");
            }
        }
        public static void LogError(object message)
        {
            foreach (Logger logger in Loggers)
            {
                logger.LogError(message ?? "Null");
            }
        }
        public static void LogException(Exception e)
        {
            foreach (Logger logger in Loggers)
            {
                logger.LogException(e);
            }
        }
    }
}
