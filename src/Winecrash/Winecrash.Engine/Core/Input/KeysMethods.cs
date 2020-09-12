using OpenTK.Input;

namespace WEngine
{
    /// <summary>
    /// All the extention methods for the <see cref="Keys"/> enum.
    /// </summary>
    public static class KeysMethods
    {
        /// <summary>
        /// Get the <see cref="Key"/> version of a <see cref="Keys"/>
        /// </summary>
        /// <param name="k"></param>
        /// <param name="isKeyboard">Is the <see cref="Keys"/> a keyboard key or mouse? If not keyboard, it returns <see cref="Key.Unknown"/>; maunally check the corresponding mouse input.</param>
        /// <returns>The corresponding <see cref="Key"/></returns>
        public static Key ToOpenTK(this Keys k, out bool isKeyboard)
        {
            isKeyboard = k.IsKeyboard();

            if (isKeyboard)
            {
                return k switch
                {
                    Keys.Back => Key.Back,
                    Keys.Tab => Key.Tab,
                    Keys.Enter => Key.Enter,
                    Keys.Menu => Key.Menu,
                    Keys.Escape => Key.Escape,
                    Keys.Space => Key.Space,
                    Keys.PageUp => Key.PageUp,
                    Keys.PageDown => Key.PageDown,
                    Keys.End => Key.End,
                    Keys.Home => Key.Home,
                    Keys.Left => Key.Left,
                    Keys.Up => Key.Up,
                    Keys.Right => Key.Right,
                    Keys.Down => Key.Down,
                    Keys.PrintScreen => Key.PrintScreen,
                    Keys.Insert => Key.Insert,
                    Keys.Delete => Key.Delete,
                    Keys.Zero => Key.Number0,
                    Keys.One => Key.Number1,
                    Keys.Two => Key.Number2,
                    Keys.Three => Key.Number3,
                    Keys.Four => Key.Number4,
                    Keys.Five => Key.Number5,
                    Keys.Six => Key.Number6,
                    Keys.Seven => Key.Number7,
                    Keys.Eight => Key.Number8,
                    Keys.Nine => Key.Number9,
                    Keys.A => Key.A,
                    Keys.B => Key.B,
                    Keys.C => Key.C,
                    Keys.D => Key.D,
                    Keys.E => Key.E,
                    Keys.F => Key.F,
                    Keys.G => Key.G,
                    Keys.H => Key.H,
                    Keys.I => Key.I,
                    Keys.J => Key.J,
                    Keys.K => Key.K,
                    Keys.L => Key.L,
                    Keys.M => Key.M,
                    Keys.N => Key.N,
                    Keys.O => Key.O,
                    Keys.P => Key.P,
                    Keys.Q => Key.Q,
                    Keys.R => Key.R,
                    Keys.S => Key.S,
                    Keys.T => Key.T,
                    Keys.U => Key.U,
                    Keys.V => Key.V,
                    Keys.W => Key.W,
                    Keys.X => Key.X,
                    Keys.Y => Key.Y,
                    Keys.Z => Key.Z,
                    Keys.LeftWindows => Key.WinLeft,
                    Keys.RightWindows => Key.WinRight,
                    Keys.Sleep => Key.Sleep,
                    Keys.NumPad0 => Key.Keypad0,
                    Keys.NumPad1 => Key.Keypad1,
                    Keys.NumPad2 => Key.Keypad2,
                    Keys.NumPad3 => Key.Keypad3,
                    Keys.NumPad4 => Key.Keypad4,
                    Keys.NumPad5 => Key.Keypad5,
                    Keys.NumPad6 => Key.Keypad6,
                    Keys.NumPad7 => Key.Keypad7,
                    Keys.NumPad8 => Key.Keypad8,
                    Keys.NumPad9 => Key.Keypad9,
                    Keys.NumPadMultiply => Key.KeypadMultiply,
                    Keys.NumPadAdd => Key.KeypadAdd,
                    Keys.NumPadSubtract => Key.KeypadSubtract,
                    Keys.NumPadDecimal => Key.KeypadDecimal,
                    Keys.NumPadDivide => Key.KeypadDivide,
                    Keys.F1 => Key.F1,
                    Keys.F2 => Key.F2,
                    Keys.F3 => Key.F3,
                    Keys.F4 => Key.F4,
                    Keys.F5 => Key.F5,
                    Keys.F6 => Key.F6,
                    Keys.F7 => Key.F7,
                    Keys.F8 => Key.F8,
                    Keys.F9 => Key.F9,
                    Keys.F10 => Key.F10,
                    Keys.F11 => Key.F11,
                    Keys.F12 => Key.F12,
                    Keys.CapsLock => Key.CapsLock,
                    Keys.NumLock => Key.NumLock,
                    Keys.ScrollLock => Key.ScrollLock,
                    Keys.LeftShift => Key.ShiftLeft,
                    Keys.RightShift => Key.ShiftRight,
                    Keys.LeftControl => Key.ControlLeft,
                    Keys.RightControl => Key.ControlRight,
                    Keys.LeftAlt => Key.AltLeft,
                    Keys.RightAlt => Key.AltRight,
                    Keys.Semicolon => Key.Semicolon,
                    Keys.Plus => Key.Plus,
                    Keys.Comma => Key.Comma,
                    Keys.Minus => Key.Minus,
                    Keys.Period => Key.Period,
                    Keys.Quotes => Key.Quote,
                    Keys.Tilde => Key.Tilde,
                    Keys.OpenBrackets => Key.BracketLeft,
                    Keys.CloseBrackets => Key.BracketRight,
                    Keys.Backslash => Key.BackSlash,
                    Keys.Clear => Key.Clear
                };
            }

            return Key.Unknown;
        }

        /// <summary>
        /// Is the key a mouse button?
        /// </summary>
        /// <param name="k">The wanted key.</param>
        /// <returns>If the key is a mouse button.</returns>
        public static bool IsMouse(this Keys k)
        {
            return k == Keys.MouseLeftButton || k == Keys.MouseRightButton || k == Keys.MouseMiddleButton || k == Keys.MouseFourthButton || k == Keys.MouseFifthButton;
        }

        /// <summary>
        /// Is the key a keyboard key?
        /// </summary>
        /// <param name="k">The wanted key.</param>
        /// <returns>If the key is a keyboard button.</returns>
        public static bool IsKeyboard(this Keys k)
        {
            return !k.IsMouse();
        }
    }
}
