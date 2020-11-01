using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using Winecrash.Entities;
using Winecrash.Net;
using Debug = WEngine.Debug;
using Graphics = WEngine.Graphics;
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
            CreateDebugWindow();

            Engine.Run(true).Wait();

            Engine.OnStop += () => End.Set();

            Debug.Log("\n\n");


            //MainLoadScreen.Show();
            
            new Sound(@"assets/sounds/button_click.mp3");
            new Shader("assets/shaders/Debuging/Volume/DebugVolume.vert", "assets/shaders/Debuging/Volume/DebugVolume.frag");
            
            Tester = new WObject("tester") /*{ Enabled = false }*/.AddModule<ClientTester>();

                                                /* ego mode enabled */
            Player.LocalPlayer = new Player("Arthur_" + new Random().Next(1000, 10000));

            Camera.Main.WObject.Delete();
            Camera.Main = null;
            
            
            
            
            WObject localPlayerWobj = new WObject("Local Player");
            localPlayerWobj.Enabled = false;
            PlayerController pc = localPlayerWobj.AddModule<PlayerController>();
            
            WObject camW = new WObject("Player Camera");
            camW.Parent = localPlayerWobj;
            camW.LocalPosition = Vector3D.Up * 1.62D;
            Camera.Main = camW.AddModule<Camera>();
            Camera.Main.FOV = 80;
            Camera.Main.NearClip = 0.01D;
            Camera.Main.FarClip = 4096.0D;
            
            
            

            GameApplication app = (GameApplication)Graphics.Window;
            app.VSync = VSyncMode.On;
            
            string title = $"Winecrash {Winecrash.Version} ({IntPtr.Size * 8}bits)";

#if DEBUG
            title += " <DEBUG BUILD>";
#endif
            
            app.Title = title;   
            
            app.OnLoaded += () =>
            {
                new Shader("assets/shaders/player/Player.vert", "assets/shaders/player/Player.frag");
                new Shader("assets/shaders/Unlit/Unlit.vert", "assets/shaders/Unlit/Unlit.frag");
                new Shader("assets/shaders/chunk/Chunk.vert", "assets/shaders/chunk/Chunk.frag");
                new Shader("assets/shaders/skybox/Skybox.vert", "assets/shaders/skybox/Skybox.frag");
                new Shader("assets/shaders/celestialbody/CelestialBody.vert", "assets/shaders/celestialbody/CelestialBody.frag");
                Database.Load("assets/items/items.json").ParseItems();
                Chunk.Texture = ItemCache.BuildChunkTexture(out int xsize, out int ysize);
                //Chunk.Texture.Save(Folders.UserData + "items_atlas.png");
                
                
                

                Canvas.Main.UICamera.FarClip = 1000.0D;

                EngineCore.Instance.WObject.AddModule<GameDebug>();

            

                Client = new GameClient();
                Client.OnDisconnected += client =>
                {
                    Game.InvokePartyLeft(PartyType.Multiplayer);
                };
                
                MainMenu.Show();
            };

            Game.OnPartyJoined += type =>
            {
                
                
                Input.LockMode = CursorLockModes.Lock;
                Input.CursorVisible = false;
                MainMenu.Hide();

                localPlayerWobj.Enabled = true;
                Player.LocalPlayer.CreateEntity(localPlayerWobj);
                
                localPlayerWobj.Position = Vector3D.Up * World.GetSurface(new Vector3D(0,0,0), "winecraft:dimension") + 1;

                if (SkyboxController.Instance)
                {
                    SkyboxController.Instance.Show();
                }
                else
                {
                    new WObject("Skybox").AddModule<SkyboxController>();
                }
                
                Player.LocalPlayer.Entity.OnRotate += rotation => 
                {
                    if (!pc.CameraLocked)
                        Camera.Main.WObject.Rotation = rotation;
                };

                if (type == PartyType.Singleplayer)
                {
                    Player.LocalPlayer.CameraAngles = Vector2I.Zero;

                    Parallel.ForEach(World.GetCoordsInRange(Vector2I.Zero, Winecrash.RenderDistance), vector =>
                    {
                        World.GetOrCreateChunk(vector, "winecrash:overworld");
                    });
                }
            };
            Game.OnPartyLeft += type =>
            {
                Input.LockMode = CursorLockModes.Free;
                Input.CursorVisible = true;
                World.Unload();

                localPlayerWobj.Enabled = false;
                Player.LocalPlayer.Entity?.Delete();
                Player.LocalPlayer.Entity = null;
                
                if (SkyboxController.Instance)
                {
                    SkyboxController.Instance.Hide();
                }

                MainMenu.Show();

                if(type == PartyType.Multiplayer)
                {
                    MainMenu.HideMain();
                }
                
                //Camera.Main._FarClip = 4096;
                //Camera.Main.WObject.Position = Vector3D.Zero;
            };


            Task.Run(() => End.WaitOne()).Wait();
        }

        private static void LogVerboseCMD(object obj)
        {

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(obj);
            Console.ResetColor();
        }
        private static void LogWarnCMD(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(obj);
            Console.ResetColor();
        }
        private static void LogErrCMD(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj);
            Console.ResetColor();
        }
        private static void LogExceptionCMD(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(obj);
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
