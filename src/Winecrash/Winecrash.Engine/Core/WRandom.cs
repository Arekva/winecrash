using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public class WRandom : Random
    {
        private static int _Count = 0;
        private static double _CountDouble = 0.0D;
        public override int Next(int maxValue)
        {
            int v = (int)(Time.TimeSinceStart * 123456789.0D * (_CountDouble + 1.0D)) % maxValue;

            if (v < 0) return -v;
            else return v;
            //return Math.Abs(((timeStartMult * _Count++ * 1024) % int.MaxValue) % maxValue);
        }

        public override int Next()
        {
            return (int)((Time.TimeSinceStart * 2E+10 * (_Count++ * 2E+10)) % 2147483647D);
        }

        public override int Next(int minValue, int maxValue)
        {
            return minValue + (int)((Time.TimeSinceStart * 2E+10 * (_Count++ * 2E+10)) % 2147483647D) % maxValue - minValue;
        }

        public override double NextDouble()
        {
            return ((Time.TimeSinceStart * 2E+10 * (_Count++ * 2E+10)) % 2147483647D) / 2147483647D;
        }
    }
}
