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

        public static RenderImage FinalImage { get; private set; } = new RenderImage(FrameResolutionX, FrameResolutionY);

        public static int FrameResolutionX { get; private set; } = 512;
        public static int FrameResolutionY { get; set; } = 512;

        private static int NextFrameResolutionX { get; set; } = FrameResolutionX;
        private static int NextFrameResolutionY { get; set; } = FrameResolutionY;

        [Initializer]
        private static void Initialize()
        {
            Updater.OnFrameEnd += InvokeRenderTargets;
            Updater.OnFrameStart += OnFrameStart;

        }

        private static void InvokeRenderTargets()
        {
            OnFrameRendered?.Invoke(FinalImage);
        }

        private static void OnFrameStart()
        {
            FrameResolutionX = NextFrameResolutionX;
            FrameResolutionY = NextFrameResolutionY;

            FinalImage = new RenderImage(FrameResolutionX, FrameResolutionY);
        }

        public static void SetNextFrameResolution(int x, int y)
        {
            NextFrameResolutionX = x;
            NextFrameResolutionY = y;
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
