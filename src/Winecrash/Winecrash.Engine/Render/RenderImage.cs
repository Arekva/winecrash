using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public sealed class RenderImage
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Bitmap Image
        {
            get
            {
                return Render.RawToBitmap(this.Data, this.Width, this.Height, Format);
            }
        }

        public const PixelFormat Format = PixelFormat.Format32bppArgb;
        public const int Canals = 4;

        private const int RedShift = 2;
        private const int GreenShift = 1;
        private const int BlueShift = 0;
        public const int AlphaShift = 3;

        private readonly byte[] Data;

        public RenderImage(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Data = new byte[width * height * Canals];
        }

        public Color32 GetPixel(int x, int y)
        {
            int i = WMath.Flatten2D(x, y, this.Width);

            //red: i * 4 + 2
            //green: i * 4 + 1
            //blue: i * 4
            //alpha: i * 4 + 3
            return new Color32(
                r: Data[i * Canals + RedShift],
                g: Data[i * Canals + GreenShift],
                b: Data[i * Canals + BlueShift],
                a: Data[i * Canals + AlphaShift]);
        }
        public void SetPixel(Color32 color, int x, int y)
        {


            int i = x + this.Width * y;

            //red: i * 4 + 2
            //green: i * 4 + 1
            //blue: i * 4
            //alpha: i * 4 + 3

            this.Data[i * Canals + RedShift] = color[0];
            this.Data[i * Canals + GreenShift] = color[1];
            this.Data[i * Canals + BlueShift] = color[2];
            this.Data[i * Canals + AlphaShift] = color[3];
        }

        public void Append(RenderImage superposed)
        {
            if (this.Data.Length != superposed.Data.Length) 
                throw new Exception("Unable the Append two RenderImage that does not have the same size.");

            for (int i = 0; i < Data.Length; i += Canals)
            {
                WMath.FlatTo2D(i, this.Width, out int x, out int y);

                Color32 colThis = this.GetPixel(x, y);
                Color32 colOther = superposed.GetPixel(x, y);

                Color32 newCol = Color.Combine(colThis, colOther);

                this.SetPixel(newCol, x, y);
            }
        }
    }
}
