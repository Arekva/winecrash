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
        /// <summary>
        /// Flashy red.
        /// </summary>
        public static Color256 Red { get; } = new Color256(1.0, 0.0, 0.0, 1.0);
        /// <summary>
        /// Flashy green.
        /// </summary>
        public static Color256 Green { get; } = new Color256(0.0, 1.0, 0.0, 1.0);
        /// <summary>
        /// Deep blue.
        /// </summary>
        public static Color256 Blue { get; } = new Color256(0.0, 0.0, 1.0, 1.0);

        /// <summary>
        /// As blue as the sky.
        /// </summary>
        public static Color256 SkyBlue { get; } = new Color256(0.529D, 0.808D, 0.922D, 1.0D);

        /// <summary>
        /// Orange like the sky. At sunset, of course.
        /// </summary>
        public static Color256 Orange { get; } = new Color256(1.0, 0.5, 0.0, 1.0);
        /// <summary>
        /// As white as the snow topping the mountain of NordVPN.
        /// </summary>
        public static Color256 White { get; } = new Color256(1.0, 1.0, 1.0, 1.0);
        /// <summary>
        /// Half white.
        /// </summary>
        public static Color256 Gray => new Color256(0.5, 0.5, 0.5, 1.0);
        /// <summary>
        /// 3/4th white.
        /// </summary>
        public static Color256 LightGray => new Color256(0.75, 0.75, 0.75, 1.0);
        /// <summary>
        /// 1/4th white.
        /// </summary>
        public static Color256 DarkGray => new Color256(0.25, 0.25, 0.25, 1.0);
        /// <summary>
        /// ⠀
        /// </summary>
        public static Color256 Transparent => new Color256(1.0D, 1.0D, 1.0D, 0.0D);
        /// <summary>
        /// The color of the NordVPN's logo (#508AFF).
        /// </summary>
        public static Color256 NordVPN => new Color256(0.314D, 0.541D, 1.0D, 1.0D);

        /// <summary>
        /// Black
        /// </summary>
        public static Color256 Black => new Color256(0.0D, 0.0D, 0.0D, 1.0D);

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
