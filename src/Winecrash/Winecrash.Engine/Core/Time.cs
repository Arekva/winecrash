using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public static class Time
    {
        // Defaults the delta time to the fixed time before updating.
        public static double DeltaTime { get; set; } = FixedDeltaTime;
        public static double TimeScale { get; set; } = 1D;


        public const double FixedDeltaTime = 1D / 16;
        public static double FixedTimeScale { get; set; } = 1D;


        internal static readonly Stopwatch ElapsedTimeWatch = new Stopwatch();
        public static double TimeSinceStart
        {
            get
            {
                return ElapsedTimeWatch.Elapsed.TotalSeconds;
            }
        }

        [Initializer]
        private static void Initialize()
        {
            ElapsedTimeWatch.Start();
        }
    }
}
