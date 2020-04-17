using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public static class Debug
    {
        private static string LogPath = FileManager.Root + @$"Logs/logs.txt";

        private static List<Logger> Loggers = new List<Logger>(1);
        
        private static List<string> logMessages = new List<string>();

        [Initializer(Int32.MinValue + 1)]
        private static void Initialize()
        {

            File.Create(LogPath);
            
            Loggers.Add(new Logger(LogFile, LogWarningFile, LogErrorFile, LogExceptionFile));

            Updater.OnFrameEnd += WriteAll;

        }

        private static bool Writing = true;
        private static void WriteAll()
        {
            if (Writing) return;
            Writing = true;
            string[] logs = logMessages.ToArray();
            logMessages.Clear();

            using(FileStream fs = new FileStream(LogPath, FileMode.Append))
                for (int i = 0; i < logs.Length; i++)
                {
                    byte[] b = Encoding.Unicode.GetBytes(logs[i] + "\n");
                    fs.Write(b, 0, b.Length);
                }
            Writing = false;
        }
        
        public static void Log(object message)
        {
            foreach (Logger logger in Loggers)
            {
                logger.Log(message);
            }
        }
        public static void LogWarning(object message)
        {
            foreach (Logger logger in Loggers)
            {
                logger.LogWarning(message);
            }
        }
        public static void LogError(object message)
        {
            foreach (Logger logger in Loggers)
            {
                logger.LogError(message);
            }
        }
        public static void LogException(Exception e)
        {
            foreach (Logger logger in Loggers)
            {
                logger.LogException(e);
            }
        }

        private static void LogFile(object message)
        {
            WriteFile("[" + Time.ShortTime + "] Info: " + message);
        }
        private static void LogWarningFile(object message)
        {
            WriteFile("[" + Time.ShortTime + "] Warning: " + message);
        }
        private static void LogErrorFile(object message)
        {
            WriteFile("[" + Time.ShortTime + "] Error: " + message);
        }
        private static void LogExceptionFile(object message)
        {
            WriteFile("[" + Time.ShortTime + "] Exception: " + message);
        }
        
        private static void WriteFile(string text)
        {
            logMessages.Add(text);
        }
    }
}
