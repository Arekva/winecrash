﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace Winecrash.Engine
{
    public static class WEngine
    {
        public static OSPlatform OS { get; private set; }

        private static OSPlatform[] SupportedOS = new OSPlatform[] { OSPlatform.Windows, OSPlatform.Linux };

        public delegate void StopDelegate();
        public static StopDelegate OnStop;

        public static void TraceLayers()
        {
            Debug.Log(Layer.GetTrace());
        }

        public static Thread Run()
        {
            Thread winThread = Viewport.ThreadRunner = new Thread(ShowWindow)
            {
                IsBackground = false,
                Priority = ThreadPriority.Highest
            };
            winThread.Start();

            Load();
            //Stop();

            return winThread;
        }

        private static void ShowWindow()
        {
            using (Viewport vp = new Viewport(800, 600, "Winecraft Viewport"))
            {
                vp.Run();
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
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Error when stopping engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!(sender is Viewport))
            {
                Viewport.ThreadRunner?.Abort();
            }

            foreach(Layer layer in Layer._Layers)
            {
                foreach(Group group in layer._Groups)
                {
                    group.Thread.Abort();
                }
            }

            Updater.UpdateThread?.Abort();
            Updater.FixedUpdateThread?.Abort();
            Debug.PrintThread?.Abort();
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
                string errorMessage = "Sorry, but Winecrash is not compatible with system " + OS.ToString();

                try
                {
                    MessageBox.Show(errorMessage, "Fatal error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    throw new Exception(errorMessage);
                }
            }
            
            Debug.Log(OS);
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