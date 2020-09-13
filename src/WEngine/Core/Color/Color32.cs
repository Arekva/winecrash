using System;

namespace WEngine
{
    /// <summary>
    /// A 32 bits RGBA Color (4 x <see cref="byte"/>) 
    /// </summary>
    [Serializable]
    public struct Color32
    {
        /// <summary>
        /// Minimal value of each of the components. Corresponds to <see cref="byte.MinValue"/>
        /// </summary>
        public const byte MinValue = Byte.MinValue;
        /// <summary>
        /// Maximal value of each of the components. Corresponds to <see cref="byte.MaxValue"/>
        /// </summary>
        public const byte MaxValue = Byte.MaxValue;


        /// <summary>
        /// Serializable red component.
        /// </summary>
        private byte _R;
        /// <summary>
        /// Red component.
        /// </summary>
        public byte R
        {
            get => _R;
            set => _R = value;
        }
        /// <summary>
        /// Serializable green component.
        /// </summary>
        private byte _G;
        /// <summary>
        /// Green component.
        /// </summary>
        public byte G
        {
            get => _G;
            set => _G = value;
        }
        /// <summary>
        /// Serializable blue component.
        /// </summary>
        private byte _B;
        /// <summary>
        /// Blue component.
        /// </summary>
        public byte B
        {
            get => _B;
            set => _B = value;
        }

        /// <summary>
        /// Serializable alpha (transparency) component.
        /// </summary>
        private byte _A;
        /// <summary>
        /// Alpha (transparency) component.
        /// </summary>
        public byte A
        {
            get => _A;
            set => _A = value;
        }

        /// <summary>
        /// Create a 32 bits color by <see cref="byte"/>s.
        /// </summary>
        /// <param name="r">The red component of the color.</param>
        /// <param name="g">The green component of the color.</param>
        /// <param name="b">The blue component of the color.</param>
        /// <param name="a">The alpha (transparency) component of the color.</param>
        public Color32(byte r, byte g, byte b, byte a)
        {
            this._R = r;
            this._G = g;
            this._B = b;
            this._A = a;
        }

        /// <summary>
        /// Create a 32 bits color by <see cref="int"/>s. Automatically clamps the values between <see cref="MinValue"/> and <see cref="MaxValue"/>.
        /// </summary>
        /// <param name="r">The red component of the color.</param>
        /// <param name="g">The green component of the color.</param>
        /// <param name="b">The blue component of the color.</param>
        /// <param name="a">The alpha (transparency) component of the color.</param>
        public Color32(int r, int g, int b, int a)
        {
            this._R = WMath.Clamp((byte)r, MinValue, MaxValue);
            this._G = WMath.Clamp((byte)g, MinValue, MaxValue);
            this._B = WMath.Clamp((byte)b, MinValue, MaxValue);
            this._A = WMath.Clamp((byte)a, MinValue, MaxValue);
        }

        /// <summary>
        /// Create a 32 bits color from a 256 bits color.
        /// </summary>
        /// <param name="color">The 256 bits color.</param>
        public static implicit operator Color32(Color256 color)
        {
            return new Color32((byte)(color.R * MaxValue), (byte)(color.G * MaxValue), (byte)(color.B * MaxValue), (byte)(color.A * MaxValue));
        }
        /// <summary>
        /// Create a 32 bits color from a 32 bits .NET color.
        /// </summary>
        /// <param name="color">The 32 bits .NET color.</param>
        public static implicit operator Color32(System.Drawing.Color color)
        {
            return new Color32(color.R, color.G, color.B, color.A);
        }

        public override string ToString()
        {
            return $"RGBA32({this.R};{this.G};{this.B};{this.A})";
        }
    }
}
