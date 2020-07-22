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
        public static Color256 Red { get; } = new Color256(1.0, 0.0, 0.0, 1.0);
        public static Color256 Green { get; } = new Color256(0.0, 1.0, 0.0, 1.0);
        public static Color256 Blue { get; } = new Color256(0.0, 0.0, 1.0, 1.0);
        public static Color256 Orange { get; } = new Color256(1.0, 0.5, 0.0, 1.0);
        public static Color256 White { get; } = new Color256(1.0, 1.0, 1.0, 1.0);

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

        public static implicit operator OpenTK.Vector4(Color256 col)
        {
            return new OpenTK.Vector4((float)col.R, (float)col.G, (float)col.B, (float)col.A);
        }

        public static implicit operator Vector4F(Color256 col)
        {
            return new Vector4F((float)col.R, (float)col.G, (float)col.B, (float)col.A);
        }

        public static implicit operator Vector4D(Color256 col)
        {
            return new Vector4D(col.R, col.G, col.B, col.A);
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
