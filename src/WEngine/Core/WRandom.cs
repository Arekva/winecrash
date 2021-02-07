using System;

namespace WEngine
{
    public class WRandom : Random
    {
        private int _Count = 0;
        private double _CountDouble = 0.0D;

        public WRandom() { }
        
        public WRandom(int seed)
        {
            _Count = seed;
            _CountDouble = seed;
        }

        public override int Next(int maxValue)
        {
            int v = (int)(Time.TimeSinceStart * 123456789.0D * (_CountDouble + 1.0D)) % maxValue;

            if (v < 0) return -v;
            else return v;
        }

        public override int Next() => (int)((Time.TimeSinceStart * 2E+10 * (_Count++ * 2E+10)) % 2147483647D);
        public override int Next(int minValue, int maxValue) => minValue + (int)((Time.TimeSinceStart * 2E+10 * (_Count++ * 2E+10)) % 2147483647D) % maxValue - minValue;
        public override double NextDouble() => ((Time.TimeSinceStart * 2E+10 * (_Count++ * 2E+10)) % 2147483647D) / 2147483647D;

        public Vector2D NextDirection2D()
        {
            double rad = WMath.Remap(NextDouble(), 0,1,-Math.PI,Math.PI);
            return new Vector2D(Math.Cos(rad), Math.Sin(rad)).Normalized;
        }
    }
}
