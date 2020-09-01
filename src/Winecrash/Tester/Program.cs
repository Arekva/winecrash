using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateDebugWindow();

            WEngine.Run().Wait();

            Graphics.Window.OnLoaded += Start;
        }

        static void Start()
        {

        }

        static void CreateDebugWindow()
        {
            Debug.AddLogger(new Logger(LogVerbose, LogWarning, LogError, LogException));
        }

        static void LogVerbose(object msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg.ToString());
        }

        static void LogWarning(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg.ToString());
        }

        static void LogError(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg.ToString());
        }

        static void LogException(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg.ToString());
        }
    }
}
