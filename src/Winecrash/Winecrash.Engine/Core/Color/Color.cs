using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public static class Color
    {
        public static Color256 Combine(Color256 a, Color256 b)
        {
            double aAB = (1.0D - a.A) * b.A + a.A;

            return new Color256(
                ((1.0D - a.A) * b.A * b.R + a.A * a.R) / aAB,
                ((1.0D - a.A) * b.A * b.G + a.A * a.G) / aAB,
                ((1.0D - a.A) * b.A * b.B + a.A * a.B) / aAB,
                aAB
            );
        }
    }
}
