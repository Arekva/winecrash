using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    //TODO : internal
    public static class Updater
    {
        internal static Thread UpdateThread;
        internal static Thread FixedUpdateThread;

        public delegate void UpdateCallback();


        public static UpdateCallback OnFrameStart;
        public static UpdateCallback OnFrameEnd;

        //[Initializer(1000)]
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

                Color32 cols = new Color32(0, 0, 0, 255);

                //Parallel.For(0, Render.FrameResolutionX * Render.FrameResolutionY, i =>
                //{
                    for (int i = 0; i < Render.FrameResolutionX * Render.FrameResolutionY; i++)
                    {
                        int x = i % Render.FrameResolutionX;
                        int y = i / Render.FrameResolutionX;
    
                        Color32 col = new Color256(
                        (double)x / Render.FrameResolutionX, 1.0D - (double)y / Render.FrameResolutionY, 0, 1.0D);
    
    
    
                        Render.FinalImage.SetPixel(col, x, y);
                    }
                //});

                //double time = Time.TimeSinceStart - timeBeforeUpdate;

                OnFrameEnd?.Invoke();

                //60 fps = 1/60th = 16 ms
                //144 fps = 1/144th = between 6ms and 7ms
                
                Thread.Sleep(16);

                Time.DeltaTime = Time.TimeSinceStart - timeBeforeUpdate;
                
                
            }
        }
    }
}
