using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows;

namespace Winecrash.Engine
{
    internal class WindowsInputWrapper : IInputWrapper
    {
        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }



        public OSPlatform CorrespondingOS { get; } = OSPlatform.Windows;

        public bool GetKey(Keys key)
        {
            return GetAsyncKeyState((System.Windows.Forms.Keys)key) != 0;
        }

        public Vector2I GetMousePosition()
        {
            GetCursorPos(out POINT lpPoint);
            return new Vector2I(lpPoint.X, lpPoint.Y);
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
    }
}
