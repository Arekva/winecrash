using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;
using Winecrash;

namespace Winecrash.Server
{
    class Test : Module
    {
        int count = 0;
        protected override void Update()
        {
            Debug.Log("update " + ++count);
        }
        protected override void Creation()
        {
            Debug.Log("tester creation");
        }
    }
    public static class Entry
    {
        public static GameServer server;

        public static void Main(string[] args)
        {
            
            CreateDebugWindow();

            ConsoleUtils.PrintSaves();

            Engine.Run(false);

            WObject wobj = new WObject("test");
            wobj.AddModule<Test>();

            server = new GameServer();
            server.Run();

            Engine.OnStop += () => Debug.Log("Engine stopped.");


            Engine.TraceLayers();


            while (true)
            {
                Console.ReadKey();
            }

            Engine.Stop();
        }

        private static void CreateDebugWindow()
        {
            Debug.AddLogger(new Logger(LogVerbose, LogWarn, LogErr, LogException));
        }

        private static void LogVerbose(object obj)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[ Verbose ] " + obj);
        }
        private static void LogWarn(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[ Warning ] " + obj);
            Console.ResetColor();
        }
        private static void LogErr(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[  Error  ] " + obj);
            Console.ResetColor();
        }
        private static void LogException(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[Exception] " + obj);
            Console.ResetColor();
        }
    }
}
