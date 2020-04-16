using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public static class Updater
    {
        internal static Thread UpdateThread;
        internal static Thread FixedUpdateThread;

        public delegate void UpdateCallback();


        public static UpdateCallback OnFrameStart;

        [Initializer]
        private static void Initialize()
        {
            //Time.ElapsedTimeWatch.Start();
            UpdateThread = new Thread(StartUpdate)
            {
                IsBackground = false,
                Priority = ThreadPriority.Highest
            };
            UpdateThread.Start();

            FixedUpdateThread = new Thread(StartUpdate)
            {
                IsBackground = false,
                Priority = ThreadPriority.Highest
            };
            FixedUpdateThread.Start();
        }

        private static void StartUpdate()
        {
            while(true)
            {
                double timeBeforeUpdate = Time.TimeSinceStart;

                OnFrameStart?.Invoke();

                //Simulates 60fps framerate
                Thread.Sleep(16);

                Time.DeltaTime = Time.TimeSinceStart - timeBeforeUpdate;
            }
        }
    }
}
