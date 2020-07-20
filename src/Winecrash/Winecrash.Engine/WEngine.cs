using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace Winecrash.Engine
{
    public static class WEngine
    {
        public static OSPlatform OS { get; private set; }

        private static OSPlatform[] SupportedOS = new OSPlatform[] { OSPlatform.Windows, OSPlatform.Linux };

        public delegate void StopDelegate();
        

        public static StopDelegate OnStop;

        public static WObject EngineObject;

        public static void TraceLayers()
        {
            Debug.Log(Layer.GetTrace());
        }

        public static Thread Run()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Thread winThread = Viewport.ThreadRunner = new Thread(ShowWindow)
            {
                IsBackground = false,
                Priority = ThreadPriority.Highest, Name = "OpenGL"
            };
            winThread.Start();

            Load();

            return winThread;
        }

        private static void ShowWindow()
        {
            Icon icon = new Icon("icon.ico");

            using (Viewport vp = new Viewport(1024, 768, "Winecrash Viewport", icon))
            {
                vp.Run(0.0D, 0.0D);
            }

        }

        public static void Load()
        {
            Initializer.InitializeEngine();
        }
        internal static void Stop(object sender)
        {
            try
            {
                OnStop?.Invoke();

                if (!(sender is Viewport))
                {
                    Viewport.ThreadRunner?.Abort();
                }

                Layer.FixedThread?.Abort();

                foreach (Layer layer in Layer._Layers)
                {
                    foreach (Group group in layer._Groups)
                    {
                        group.Thread.Abort();
                    }
                }
            }

            catch(Exception e)
            {
                Debug.LogError("Error when stopping engine: " + e);
            }
        }
        public static void Stop()
        {
            Stop(null);
        }

        [Initializer(Int32.MinValue + 10)]
        private static void Initialize()
        {
            CheckPlateform();

            if(!SupportedOS.Contains(OS))
            {
                Debug.LogError("Sorry, but Winecrash is not compatible with system " + OS.ToString());
            }

            CreateEngineWObject();

            GUI.Font pixelized = new GUI.Font("assets/fonts/pixelized.json", "Pixelized");

           /* for (int i = 0; i < pixelized.Glyphs.Set.Length; i++)
            {
                Debug.LogWarning(pixelized.Glyphs[pixelized.Glyphs.Set[i]].Character);
            }*/
        }

        private static WObject CreateEngineWObject()
        {
            WObject wobj = new WObject("Engine Core")
            {
                Undeletable = true
            };

            wobj.AddModule<Input>().ExecutionOrder = Int32.MinValue;
            wobj.AddModule<EngineCore>();

            WObject wobjcan = new WObject("Canvas");
            wobjcan.AddModule<GUI.Canvas>();
            

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
    }
}
