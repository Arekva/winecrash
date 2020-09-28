using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using WEngine;
using WEngine.Networking;
using Winecrash;
using Winecrash.Net;

namespace Winecrash.Server
{
    public static class Entry
    {
        public static GameServer server;

        public static void Main(string[] args)
        {
            CreateDebugWindow();

            //Folders.UserData = "Data/";

            //new Save(Save.DefaultName, false);

            //ConsoleUtils.PrintSaves();

            Engine.Run(false);
            
            Database.Load("assets/items/items.json").ParseItems();

            server = new GameServer(IPAddress.Any, 27716);

            server.OnPlayerConnect += (player) =>
            {
                Debug.Log("Sending debug chunks");

                Parallel.ForEach(World.GetCoordsInRange(Vector2I.Zero, 15), (vector) =>
                {
                    NetObject.Send(new NetChunk(World.GetOrCreateChunk(vector, "winecrash:overworld")),
                        player.Client.Client);
                });

                //for (int i = 0; i < 256; i++)
                //{
                //World.GetOrCreateChunk(new Vector2I(0, i), "winecrash:overworld");
                //}
            };

            server.OnPlayerDisconnect += (player, reason) =>
            {
                //WObject.TraceHierarchy();
                //World.WorldWObject?.Delete();
            };
            
            server.Run();

            //Engine.OnStop += () => Debug.Log("Engine stopped.");


            Console.ReadKey();
            //Engine.Stop();
        }

        private static void CreateDebugWindow()
        {
            Debug.AddLogger(new Logger(LogVerbose, LogWarn, LogErr, LogException));
        }

        private static void LogVerbose(object obj)
        {

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[ Verbose ] " + obj);
            Console.ResetColor();
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
