using System;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.IO;

namespace WEngine
{
    /// <summary>
    /// The delegate used by <see cref="Engine.OnStop"/>.
    /// </summary>
    public delegate void WEngineStopDelegate();

    public static class Engine
    {
        /// <summary>
        /// The Operating System running the program.
        /// </summary>
        public static OSPlatform OS { get; private set; }
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
        public static WObject EngineObject { get; private set; } = null;

        public static bool DoGUI { get; private set; }

        public static void TraceLayers()
        {
            Debug.Log(Layer.GetTrace());
        }

        public async static Task Run(bool gui)
        {
            DoGUI = gui;

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
            if(Engine.OS == OSPlatform.Windows && File.Exists("assets/icon.ico"))
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
            OnStop?.Invoke();

            if (!(sender is IWindow window))
            {
                Graphics.Window.Thread?.Abort();
            }

            Layer.FixedThread?.Abort();

            foreach (Layer layer in Layer._Layers)
            {
                foreach (Group group in layer._Groups)
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
            CheckPlateform();

            if(!SupportedOS.Contains(OS))
            {
                Debug.LogError("Sorry, but Winecrash is not compatible with system " + OS.ToString());
            }

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

        private static void CheckPlateform()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OS = OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OS = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OS = OSPlatform.OSX;
            }
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
