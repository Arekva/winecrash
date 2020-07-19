using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenTK.Input;


namespace Winecrash.Engine
{
    internal class LinuxInputWrapper : IInputWrapper
    {
        public OSPlatform CorrespondingOS { get; } = OSPlatform.Linux;
        bool[] debugged = new bool[3];
        public bool GetKey(Keys key)
        {
            Key tkKey = key.ToOpenTK(out bool isActualKey);

            return isActualKey ? Keyboard.GetState().IsKeyDown(tkKey) : false;
        }

        public Vector2I GetMousePosition()
        {
            MouseState ms = Mouse.GetCursorState();

            return new Vector2I(ms.X, ms.Y);
        }

        public void SetMousePosition(Vector2I position)
        {
            Mouse.SetPosition(position.X, position.Y);
        }
    }
}
