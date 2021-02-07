using System;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using JCS;

namespace WEngine
{
    /// <summary>
    /// The delegate used by <see cref="Engine.OnStop"/>.
    /// </summary>
    public delegate void WEngineStopDelegate();

    public static class Engine
    {
        /// <summary>
        /// The engine version running.
        /// </summary>
        public static Version Version { get; } = new Version(0, 0, 1, "Stève");

        public const string SourceLink = "https://github.com/breaks-and-continues/winecrash";
        
        public static string[] Arguments { get; internal set; }

        /// <summary>
        /// The OS running the engine.
        /// </summary>
        public static OS OS { get; } = OS.FromCurrentConfig();

        /// <summary>
        /// The current CPU running the program ~~or not~~
        /// </summary>
        public static CPU CPU { get; } = CPU.FromCurrentConfig();

        public static GPU GPU { get; } = GPU.FromCurrentConfig();

        public static RAM RAM { get; } = RAM.FromCurrentConfig();
        
        /// <summary>
        /// Are Single-Instruction-Multiple-Data available?
        /// </summary>
        //public static bool IsSIMDAvailable { get; } = Vector.IsHardwareAccelerated;

        /// <summary>
        /// All Operating Systems the engine supports. MacOS should come with NET 5.0.
        /// </summary>
        public static OSPlatform[] SupportedOS { get; } = new OSPlatform[] { OSPlatform.Windows, OSPlatform.Linux };

        /// <summary>
        /// Triggered when the <see cref="WEngine"/> stops.
        /// </summary>
        public static event WEngineStopDelegate OnStop;

        /// <summary>
        /// The <see cref="WObject"/> of the Engine. It is undestructible and manages things like the Input.
        /// </summary>
        private static WObject EngineObject { get; set; } = null;

        public static bool Running { get; private set; } = false;
        
        public static bool DoGUI { get; private set; }

        public static void TraceLayers()
        {
            Debug.Log(Layer.GetTrace());
        }

        public async static Task Run(bool gui, string[] args)
        {
            Running = true;
            DoGUI = gui;

            Arguments = args;

            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            if (DoGUI)
            {
                await Task.Run(() =>
                {
                    Thread winThread = new Thread(ShowWindow)
                    {
                        IsBackground = false,
                        Priority = ThreadPriority.Highest,
                        Name = "OpenGL"
                    };

                    winThread.Start();
                    windowSet.WaitOne();

                    Load();
                });
            }
            else
            {
                Load();
            }
        }
        private static ManualResetEvent windowSet = new ManualResetEvent(false);

        private static void ShowWindow()
        {
            Icon icon = null;
            
            // mono dislikes the icon constructor...
            // TODO: change that under NET 5.0
            if(Engine.OS.Platform == OSPlatform.Windows && File.Exists("assets/icon.ico"))
            {
                icon = new Icon("assets/icon.ico");
            }

            using (GameApplication app = new GameApplication("Winecrash Viewport", icon))
            {
                Graphics.Window = app;
                windowSet.Set();
                Graphics.Window.Thread = Thread.CurrentThread;
                app.Run(0.0D, 0.0D);
            }
        }

        public static void Load()
        {
            Initialize();
            
            
            Time.Initialize();
            Layer.Initialize();
            //Initializer.InitializeEngine();
        }
        internal static void Stop(object sender)
        {
            Engine.Running = false;
            //Layer.UpdateRenderEvent.Set();
            Layer.PhysicsEvent.Set();
            OnStop?.Invoke();

            if (!(sender is IWindow window))
            {
                Graphics.Window.Thread?.Abort();
            }

            //Layer.FixedThread?.Abort();

            foreach (Layer layer in Layer._Layers)
            {
                lock (layer.Groups)
                {
                    foreach (Group group in layer.Groups)
                    {
                        try
                        {
                            group.Thread.Abort();
                            
                        }
                        catch (Exception e)
                        {
/*#IF DEBUG
                        Debug.LogException(e);
#ENDIF*/
                        }
                        try
                        {
                            group.PhysicsThread.Abort();
                            
                        }
                        catch (Exception e)
                        {
/*#IF DEBUG
                        Debug.LogException(e);
#ENDIF*/
                        }
                    }

                    layer.Groups = null;
                }
            }

            Networking.BaseServer[] servers = Networking.BaseServer.Servers.ToArray();

            for (int i = 0; i < servers.Length; i++)
            {
                servers[i].Delete();
            }

            servers = null;
        }
        public static void Stop()
        {
            Stop(null);
        }

        //[Initializer(Int32.MinValue + 10)]
        private static void Initialize()
        {
            DebugInfos();

            if (!SupportedOS.Contains(OS.Platform))
            {
                throw new EngineException("Sorry, but Winecrash is not compatible with system " + OS.ToString());
            }

            /*if(!IsSIMDAvailable)
            {
                Debug.LogWarning("SIMD is not supported on this system. The game might run slower.");
            }*/

            if (DoGUI)
            {
                CreateEngineWObject();
            }
            else
            {
                Layer.CreateOrGetLayer(0).Name = "Default Layer";
                //Group.CreateOrGetGroup(0, "Default Group");
            }
        }
        
        private static void DebugInfos()
        {
            string engine = "WEngine " + Version.ToString("{M}.{m}.{p} \"{n}\"");
            string os = OS.Version.Name + " x" + OS.Architecture + " (" + OS.Version.ToString("{M}.{m}.{p}") + ")";
            string cpu = CPU.Name;
            string gpu = GPU.Name;
            string ram = (RAM.Capacity / 1_000_000UL).ToString() + "MB";
            
            Debug.Log(engine + 
                      Environment.NewLine + SourceLink + 
                      Environment.NewLine + Environment.NewLine + 
                      "---- Specifications --------------" +
                      Environment.NewLine + "OS : " + os + 
                      Environment.NewLine + "CPU: " + cpu + 
                      Environment.NewLine + "GPU: " + gpu + 
                      Environment.NewLine + "RAM: " + ram +
                      Environment.NewLine);
        }

        private static WObject CreateEngineWObject()
        {
            WObject wobj = new WObject("Engine Core")
            {
                Undeletable = true
            };

            wobj.AddModule<Input>().ExecutionOrder = Int32.MinValue;
            wobj.AddModule<EngineCore>();

            if (DoGUI)
            {
                WObject wobjcan = new WObject("Canvas")
                {
                    Undeletable = true
                };
                wobjcan.AddModule<GUI.Canvas>();
            }
            

            Layer.CreateOrGetLayer(0).Name = "Default Layer";
            Group.CreateOrGetGroup(-1, "3D Logic");

            EngineObject = wobj;
            return wobj;
        }

        private static double _TimeSinceLastUpdate = 0.0D;
        private static bool _FirstRun = false;


        public static void ForceUpdate()
        {
            if(_FirstRun)
            {
                _TimeSinceLastUpdate = Time.TimeSinceStart;
            }

            double delta = Time.TimeSinceStart - _TimeSinceLastUpdate;
            Layer.Update(new UpdateEventArgs(delta));
        }
    }
}
