using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public struct Color256
    {
        public const double MinValue = 0.0D;
        public const double MaxValue = 1.0D;

        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double A { get; set; }

        public Color256(double r, double g, double b, double a)
        {
            this.R = WMath.Clamp(r, MinValue, MaxValue);
            this.G = WMath.Clamp(g, MinValue, MaxValue);
            this.B = WMath.Clamp(b, MinValue, MaxValue);
            this.A = WMath.Clamp(a, MinValue, MaxValue);
        }

        public Color256(Color32 colour)
        {
            this.R = (double)colour.R / Color32.MaxValue;
            this.G = (double)colour.G / Color32.MaxValue;
            this.B = (double)colour.B / Color32.MaxValue;
            this.A = (double)colour.A / Color32.MaxValue;
        }

        public static implicit operator Color256(Color32 colour)
        {
            return new Color256(colour);
        }

        public static implicit operator Color256(System.Drawing.Color colour)
        {
            return new Color256(colour);
        }

        public static Color256 operator *(Color256 c, double v)
        {
            return new Color256(c.R * v, c.G * v, c.B * v, c.A * v);
        }

        public override string ToString()
        {
            return $"RGBA256({this.R};{this.G};{this.B};{this.A})";
        }
    }
}
