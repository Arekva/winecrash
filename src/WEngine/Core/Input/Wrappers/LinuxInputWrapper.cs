using System.Runtime.InteropServices;
using OpenTK.Input;


namespace WEngine
{
    /// <summary>
    /// The <see cref="OSPlatform.Linux"/> input wrapper.
    /// </summary>
    internal class LinuxInputWrapper : IInputWrapper
    {
        public OSPlatform CorrespondingOS { get; } = OSPlatform.Linux;

        public bool GetKey(Keys key)
        {
            Key tkKey = key.ToOpenTK(out bool isActualKey);

            if(isActualKey)
            {
                return Keyboard.GetState().IsKeyDown(tkKey);
            }
            else
            {
                MouseButton button = MouseButton.Left;
                bool found = true;

                switch(key)
                {
                    case Keys.MouseLeftButton:
                        button = MouseButton.Left;
                        break;

                    case Keys.MouseRightButton:
                        button = MouseButton.Right;
                        break;
                    
                    case Keys.MouseMiddleButton:
                        button = MouseButton.Middle;
                        break;
                    
                    case Keys.MouseFourthButton:
                        button = MouseButton.Button4;
                        break;

                    case Keys.MouseFifthButton:
                        button = MouseButton.Button5;
                        break;

                    default:
                        found = false;
                        break;
                }

                
                return found && OpenTK.Input.Mouse.GetState()[button];
            }
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
