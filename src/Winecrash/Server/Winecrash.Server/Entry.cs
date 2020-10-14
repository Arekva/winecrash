using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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


            Folders.UserData = "Data/";
            ConsoleUtils.PrintSaves();


            //new Save(Save.DefaultName, false);



            Engine.Run(false).Wait();
            
            Database.Load("assets/items/items.json").ParseItems();

            try
            {
                WEngine.Debug.Log("Save \"save\" found. Loading.");
                Server.Save = new Save("save", true);
            }
            catch (SaveException se)
            {
                WEngine.Debug.Log("Save \"save\" not found. Creating.");
                Server.Save = new Save("save", false);
            }

            Stopwatch worldLoadSW = new Stopwatch();         
            WEngine.Debug.Log("Loading world...");
            worldLoadSW.Start();
            Parallel.ForEach(World.GetCoordsInRange(Vector2I.Zero, 15), vector =>
            {
                World.GetOrCreateChunk(vector, "winecrash:overworld");
            });
            worldLoadSW.Stop();
            WEngine.Debug.Log($"World loaded ! ({worldLoadSW.Elapsed.TotalMilliseconds:F0} ms)");

            server = new GameServer(IPAddress.Any, 27716);

            server.OnPlayerConnect += player =>
            {
                Task.Run(() =>
                {
                    Parallel.ForEach(World.GetCoordsInRange(Vector2I.Zero, 15), vector =>
                    {
                        if (!player.Connected) return;
                        
                        new NetChunk(World.GetOrCreateChunk(vector, "winecrash:overworld")).Send(
                            player.Client.Client);
                    });
                });
            };

            server.OnPlayerDisconnect += (player, reason) =>
            {
                //World.Unload();
                //GC.Collect();
            };
            
            server.Run();

            //Engine.OnStop += () => Debug.Log("Engine stopped.");


            Console.ReadKey();
            //Engine.Stop();
        }

        private static void CreateDebugWindow()
        {
            WEngine.Debug.AddLogger(new Logger(LogVerbose, LogWarn, LogErr, LogException));
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
