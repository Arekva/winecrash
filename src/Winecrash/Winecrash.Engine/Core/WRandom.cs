using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public class WRandom : Random
    {
        private static ulong _Count = 0ul;
        public override int Next(int maxValue)
        {
            return (int)((Time.TimeSinceStart * 123456789D * (_Count++ * 987654321D)) % 2147483647D) % maxValue;
        }

        public override int Next()
        {
            return (int)((Time.TimeSinceStart * 123456789D * (_Count++ * 987654321D)) % 2147483647D);
        }

        public override int Next(int minValue, int maxValue)
        {
            return minValue + (int)((Time.TimeSinceStart * 123456789D * (_Count++ * 987654321D)) % 2147483647D) % maxValue - minValue;
        }

        public override double NextDouble()
        {
            return ((Time.TimeSinceStart * 123456789D * (_Count++ * 987654321D)) % 2147483647D) / 2147483647D;
        }
    }
}
