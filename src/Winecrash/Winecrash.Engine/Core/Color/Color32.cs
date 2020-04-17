using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public struct Color32
    {
        public const byte MinValue = Byte.MinValue;
        public const byte MaxValue = Byte.MaxValue;

        public byte R { get; private set; }
        public byte G { get; private set; }
        public byte B { get; private set; }
        public byte A { get; private set; }

        public byte this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return R;
                    case 1: return G;
                    case 2: return B;
                    case 3: return A;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public Color32(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
        public Color32(int r, int g, int b, int a)
        {
            this.R = (byte)WMath.Clamp(r, MinValue, MaxValue);
            this.G = (byte)WMath.Clamp(g, MinValue, MaxValue);
            this.B = (byte)WMath.Clamp(b, MinValue, MaxValue);
            this.A = (byte)WMath.Clamp(a, MinValue, MaxValue);
        }
        public Color32(Color256 colour)
        {
            this.R = (byte)(colour.R * MaxValue);
            this.G = (byte)(colour.G * MaxValue);
            this.B = (byte)(colour.B * MaxValue);
            this.A = (byte)(colour.A * MaxValue);
        }

        public Color32(System.Drawing.Color colour)
        {
            this.R = colour.R;
            this.G = colour.G;
            this.B = colour.B;
            this.A = colour.A;
        }

        public static implicit operator Color32(Color256 colour)
        {
            return new Color32(colour);
        }

        public static implicit operator Color32(System.Drawing.Color colour)
        {
            return new Color32(colour);
        }

        public static implicit operator byte[](Color32 colour)
        {
            return new byte[3] { colour.A, colour.G, colour.R };
        }

        public override string ToString()
        {
            return $"RGBA32({this.R};{this.G};{this.B};{this.A})";
        }
    }
}
