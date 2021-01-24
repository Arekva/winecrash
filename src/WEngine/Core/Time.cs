using System;
using System.Diagnostics;

namespace WEngine
{
    /// <summary>
    /// Time management state machine, contains various members to play with time.
    /// </summary>
    public static class Time
    {
#region Time (Unfixed)
        // Default values for time
        private const double DefaultDelta = 1.0D / 60.0D;
        private const double DefaultScale = 1.0D;
        
        /// <summary>
        /// The scale (speed) of the time.
        /// </summary>
        public static double Scale { get; set; } = DefaultScale;
        /// <summary>
        /// The unscaled time elapsed since the last frame, in seconds.
        /// </summary>
        public static double DeltaUnscaled { get; internal set; } = DefaultDelta;
        /// <summary>
        /// The scaled time elapsed since the last frame began, in seconds.
        /// <br>Equivalent of <see cref="DeltaUnscaled"/>x<see cref="Scale"/></br>
        /// </summary>
        public static double Delta => Scale * DeltaUnscaled;
#endregion

#region Physics Time (Fixed)
        // Default values for physics time
        private const double DefaultPhysicsDelta = 1.0D / 60.0D;
        private const double DefaultPhysicsRate = 60.0D;
        private const double DefaultPhysicsScale = 1.0D;

        /// <summary>
        /// The physics clock rate, in Hz.
        /// </summary>
        public static double PhysicsRate { get; set; } = DefaultPhysicsRate;
        /// <summary>
        /// The time between two physics frame, in seconds.
        /// </summary>
        internal static double PhysicsRateInverted => 1.0 / PhysicsRate;
        /// <summary>
        /// The scale of the physics time.
        /// </summary>
        public static double PhysicsScale { get; set; } = DefaultPhysicsScale;

        // backing field for PhysicsDeltaUnscaled
        private static double _physicsDeltaUnscaled = DefaultPhysicsDelta;

        /// <summary>
        /// The unscaled physics time elapsed since the last physics frame began, in seconds.
        /// </summary>
        public static double PhysicsDeltaUnscaled
        {
            get => _physicsDeltaUnscaled;
            set => _physicsDeltaUnscaled = value; //Math.Max(value, PhysicsRateInverted);
        }

        /// <summary>
        /// The scaled physics time elapsed since the last physics frame began, in seconds.
        /// <br>Equivalent of <see cref="PhysicsDeltaUnscaled"/>x<see cref="PhysicsScale"/></br>
        /// </summary>
        public static double PhysicsDelta => PhysicsDeltaUnscaled * PhysicsScale;
#endregion
        

        /*public static double FixedDeltaTime {get; internal set; } = 1D / 60;
        public static double FixedTimeScale { get; set; } = 1D;*/
        
        internal static Stopwatch FrameTimer { get; set; } = new Stopwatch();

        public static ulong Frame { get; internal set; } = 0UL;


        internal static double TimeSinceRenderBegan => _RenderSW.Elapsed.TotalSeconds;

        private static Stopwatch _RenderSW = new Stopwatch();
        internal static void BeginRender() => _RenderSW.Start();
        
        internal static void EndRender() => _RenderSW.Stop();
        
        internal static void ResetRender() => _RenderSW.Reset();

        public static string ShortTime
        {
            get
            {
                DateTime now = DateTime.Now;

                return $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}";
            }
        }
        
        public static string ShortTimeForFile
        {
            get
            {
                DateTime now = DateTime.Now;

                return $"{now.Hour:D2}_{now.Minute:D2}_{now.Second:D2}";
            }
        }


        internal static readonly Stopwatch ElapsedTimeWatch = new Stopwatch();
        public static double TimeSinceStart => ElapsedTimeWatch.Elapsed.TotalSeconds;

        //[Initializer]
        internal static void Initialize()
        {
            ElapsedTimeWatch.Start();
        }
    }
}
