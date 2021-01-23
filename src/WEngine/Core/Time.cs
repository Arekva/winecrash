using System;
using System.Diagnostics;

namespace WEngine
{
    public static class Time
    {
        // Defaults the delta time to the fixed time before updating.
        private static double _DeltaTime = 1 / 60D;

        public static double DeltaTime
        {
            get
            {
                return _DeltaTime;
            }
            internal set
            {
                _DeltaTime = value;
                PhysicsDeltaTime = Math.Min(value, 1 / 60D);
            }
        }
        public static double PhysicsDeltaTime { get; private set; } = 1 / 60D;
        public static double TimeScale { get; set; } = 1D;

        /*public static double FixedDeltaTime {get; internal set; } = 1D / 60;
        public static double FixedTimeScale { get; set; } = 1D;*/
        
        internal static Stopwatch FrameTimer { get; set; } = new Stopwatch();

        public static ulong Frame { get; internal set; } = 0UL;


        internal static double TimeSinceRenderBegan
        {
            get
            {
                return _RenderSW.Elapsed.TotalSeconds;
            }
        }

        private static Stopwatch _RenderSW = new Stopwatch();
        internal static void BeginRender()
        {
            _RenderSW.Start();
        }
        internal static void EndRender()
        {
            _RenderSW.Stop();
        }
        internal static void ResetRender()
        {
            _RenderSW.Reset();
        }

        public static string ShortTime
        {
            get
            {
                DateTime now = DateTime.Now;

                return $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}";
            }
        }
        
        internal static string ShortTimeForFile
        {
            get
            {
                DateTime now = DateTime.Now;

                return $"{now.Hour:D2}_{now.Minute:D2}_{now.Second:D2}";
            }
        }


        internal static readonly Stopwatch ElapsedTimeWatch = new Stopwatch();
        public static double TimeSinceStart
        {
            get
            {
                return ElapsedTimeWatch.Elapsed.TotalSeconds;
            }
        }

        //[Initializer]
        internal static void Initialize()
        {
            ElapsedTimeWatch.Start();
        }
    }
}
