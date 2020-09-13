using System;

namespace WEngine
{
    /// <summary>
    /// A 256 bits RGBA Color (4 x <see cref="double"/>) 
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// Serializable red component.
        /// </summary>
        private double _R;
        /// <summary>
        /// Red component.
        /// </summary>
        public double R
        {
            get => _R;
            set => _R = value;
        }
        /// <summary>
        /// Serializable green component.
        /// </summary>
        private double _G;
        /// <summary>
        /// Green component.
        /// </summary>
        public double G
        {
            get => _G;
            set => _G = value;
        }
        /// <summary>
        /// Serializable blue component.
        /// </summary>
        private double _B;
        /// <summary>
        /// Blue component.
        /// </summary>
        public double B
        {
            get => _B;
            set => _B = value;
        }

        /// <summary>
        /// Serializable alpha (transparency) component.
        /// </summary>
        private double _A;
        /// <summary>
        /// Alpha (transparency) component.
        /// </summary>
        public double A
        {
            get => _A;
            set => _A = value;
        }

        /// <summary>
        /// Create a color from percentage values (i.e. 1.0 of red will be bright red while 0.2 will be dark red).
        /// </summary>
        /// <param name="r">The red component of the color.</param>
        /// <param name="g">The green component of the color.</param>
        /// <param name="b">The blue component of the color.</param>
        /// <param name="a">The alpha (transparency) component of the color.</param>
        public Color256(double r, double g, double b, double a)
        {
            this._R = r;
            this._G = g;
            this._B = b;
            this._A = a;
        }

        /// <summary>
        /// Converts a <see cref="Color32"/> (32 bit) color into a <see cref="Color256"/> (256 bit) color.
        /// </summary>
        /// <param name="colour">The 32 bit color to convert.</param>
        public static implicit operator Color256(Color32 colour)
        {
            return new Color256(colour.R, colour.G, colour.B, colour.A);
        }

        /// <summary>
        /// Converts a .NET <see cref="System.Drawing.Color"/> (32 bit) color into a <see cref="Color256"/> (256 bit) color.
        /// </summary>
        /// <param name="colour">The 32 bit .NET color to convert.</param>
        public static implicit operator Color256(System.Drawing.Color colour)
        {
            return new Color256(colour.R / 255.0D, colour.G / 255.0D, colour.B / 255.0D, colour.A / 255.0D);
        }

        /// <summary>
        /// Converts a <see cref="Color256"/> (256 bit) color into an OpenTK float <see cref="OpenTK.Vector4"/>.
        /// </summary>
        /// <param name="colour">The 256 color to convert.</param>
        public static implicit operator OpenTK.Vector4(Color256 col)
        {
            return new OpenTK.Vector4((float)col.R, (float)col.G, (float)col.B, (float)col.A);
        }

        /// <summary>
        /// Converts a <see cref="Color256"/> (256 bit) color into a Vector4F <see cref="Vector4F"/>.
        /// </summary>
        /// <param name="colour">The 256 color to convert.</param>
        public static implicit operator Vector4F(Color256 col)
        {
            return new Vector4F((float)col.R, (float)col.G, (float)col.B, (float)col.A);
        }

        /// <summary>
        /// Converts a <see cref="Color256"/> (256 bit) color into a Vector4D <see cref="Vector4D"/>.
        /// </summary>
        /// <param name="colour">The 256 color to convert.</param>
        public static implicit operator Vector4D(Color256 col)
        {
            return new Vector4D(col.R, col.G, col.B, col.A);
        }

        /// <summary>
        /// Multiply a color by a value.
        /// </summary>
        /// <param name="c">The color.</param>
        /// <param name="v">The multiplication factor.</param>
        /// <returns>A multiplied <see cref="Color256"/></returns>
        public static Color256 operator *(Color256 c, double v)
        {
            return new Color256(c.R * v, c.G * v, c.B * v, c.A * v);
        }

        /// <summary>
        /// Multiply two colors together.
        /// </summary>
        /// <param name="c">The first color.</param>
        /// <param name="v">The second color.</param>
        /// <returns>A multiplied <see cref="Color256"/></returns>
        public static Color256 operator *(Color256 c, Color256 v)
        {
            return new Color256(c.R * v.R, c.G * v.G, c.B * v.B, c.A * v.A);
        }

        public override string ToString()
        {
            return $"RGBA256({this.R};{this.G};{this.B};{this.A})";
        }
    }
}
