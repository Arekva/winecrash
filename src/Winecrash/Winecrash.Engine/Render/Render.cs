using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public static class Render
    {
        public delegate void FrameRenderDelegate(RenderImage image);
        public static FrameRenderDelegate OnFrameRendered;

        internal static RenderImage FinalImage { get; private set; } = new RenderImage(512, 512);

        [Initializer]
        private static void Initialize()
        {
            Updater.OnFrameEnd += InvokeRenderTargets;
        }

        private static void InvokeRenderTargets()
        {
            OnFrameRendered?.Invoke(FinalImage);
        }

        

        internal static Bitmap RawToBitmap(byte[] pixels, int width, int height, PixelFormat format)
        {
            Bitmap bmp = new Bitmap(width, height, format);

            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
