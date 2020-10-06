using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WEngine;
using WEngine.GUI;
using WEngine.Networking;
using Winecrash;
using Winecrash.Net;
using Debug = WEngine.Debug;
using Label = WEngine.GUI.Label;

namespace Winecrash.Client
{
    static class Program
    {
        private static ManualResetEvent End = new ManualResetEvent(false);

        private static Label LbDebug;

        public static GameClient Client;

        public static ClientTester Tester;

        static void Main(string[] args)
        {
            Engine.Run(true).Wait();

            Engine.OnStop += () => End.Set();

            CreateDebugWindow();
            
            new Sound(@"assets/sounds/button_click.mp3");
            
            Tester = new WObject("tester") /*{ Enabled = false }*/.AddModule<ClientTester>();

            Client = new GameClient("Arthur");

            

            Client.OnConnected += client =>
            {
                Camera.Main._FarClip = 4096;
                Camera.Main.WObject.Position = Vector3D.Up * 512;

                Task.Run(() =>
                {
                    while (true)
                    {
                        Camera.Main.WObject.Rotation = new Quaternion(90, 0, 0);
                        Task.Delay(1).Wait();
                    }
                });
               
            };

            GameApplication app = (GameApplication)Graphics.Window;
            app.OnLoaded += () =>
            {
                new Shader("assets/shaders/chunk/Chunk.vert", "assets/shaders/chunk/Chunk.frag");
                app.Title = "Winecrash " + IntPtr.Size * 8 + "bits";
                Database.Load("assets/items/items.json").ParseItems();
                Chunk.Texture = ItemCache.BuildChunkTexture(out int xsize, out int ysize);
                MainMenu.Show();
            };
            
            
            Task.Run(() => End.WaitOne()).Wait();
        }

        private static void LogVerboseCMD(object obj)
        {

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[ Verbose ] " + obj);
            Console.ResetColor();
        }
        private static void LogWarnCMD(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[ Warning ] " + obj);
            Console.ResetColor();
        }
        private static void LogErrCMD(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[  Error  ] " + obj);
            Console.ResetColor();
        }
        private static void LogExceptionCMD(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[Exception] " + obj);
            Console.ResetColor();
        }

        private static void CreateDebugWindow()
        {
            Debug.AddLogger(new Logger(LogVerbose, LogWarning, LogError, LogException));
            Debug.AddLogger(new Logger(LogVerboseCMD, LogWarnCMD, LogErrCMD, LogExceptionCMD));
        }
        
        private static void LogVerbose(object obj)
        {
            if (LbDebug == null) return;
            string msg = "\nVerbose   - " + (obj == null ? "Null" : obj.ToString());
            LbDebug.Text += msg;
        }

        private static void LogWarning(object obj)
        {
            if (LbDebug == null) return;

            string msg = "\nWarning   - " + (obj == null ? "Null" : obj.ToString());
            LbDebug.Text += msg;
        }

        private static void LogError(object obj)
        {
            if (LbDebug == null) return;

            string msg = "\nError     - " + (obj == null ? "Null" : obj.ToString());
            LbDebug.Text += msg;
        }

        private static void LogException(object obj)
        {
            if (LbDebug == null) return;

            string msg = "\nExeception- " + (obj == null ? "Null" : obj.ToString());
            LbDebug.Text += msg;
        }
    }
}
