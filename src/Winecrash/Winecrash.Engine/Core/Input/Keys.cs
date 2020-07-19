using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace Winecrash.Engine
{
    public static class KeysMethods
    {
        public static Key ToOpenTK(this Keys k, out bool isKeyboard)
        {
            isKeyboard = k.IsKeyboard();

            if(isKeyboard)
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

        public static bool IsMouse(this Keys k)
        {
            return k == Keys.MouseLeftButton || k == Keys.MouseRightButton || k == Keys.MouseMiddleButton || k == Keys.MouseFourthButton || k == Keys.MouseFifthButton;
        }

        public static bool IsKeyboard(this Keys k)
        {
            return !k.IsMouse();
        }
    }
    

    [Flags]
    public enum Keys
    {
        MouseLeftButton = 1,
        MouseRightButton = 2,
        MouseMiddleButton = 4,
        MouseFourthButton = 5,
        MouseFifthButton = 6,

        //Cancel = 3,
        Back = 8,
        Tab = 9,
        //LineFeed = 10,

        //Clear = 12,
        //Return = 13,
        Enter = 13,
        //Shift = 16,
        //Control = 17,
        Menu = 18,

        Escape = 27,
        Space = 32,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,

        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,

        //Select = 41,
        //Print = 42,
        //Execute = 43,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        //Help = 47,


        Zero = 48,
        One = 49,
        Two = 50,
        Three = 51,
        Four = 52,
        Five = 53,
        Six = 54,
        Seven = 55,
        Eight = 56,
        Nine = 57,

        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,


        LeftWindows = 91,
        RightWindows = 92,
        //Apps = 93,
        Sleep = 95,


        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        NumPadMultiply = 106,
        NumPadAdd = 107,
        NumPadSubtract = 109,
        NumPadDecimal = 110,
        NumPadDivide = 111,

        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,

        CapsLock = 20,
        NumLock = 144,
        ScrollLock = 145,

        LeftShift = 160,
        RightShift = 161,
        LeftControl = 162,
        RightControl = 163,
        LeftAlt = 164,
        RightAlt = 165,

        //VolumeMute = 173,
        //VolumeDown = 174,
        //VolumeUp = 175,

        //MediaNextTrack = 176,
        //MediaPreviousTrack = 177,
        //MediaPlayPause = 179,

        
        
        
        

        Semicolon = 186, //Oem1 = 186,
        Plus = 187,
        Comma = 188,
        Minus = 189,
        Period = 190,
        //Question = 191, //Oem2 = 191,
        Tilde = 192, //Oem3 = 192,
        OpenBrackets = 219, //Oem4 = 219,
        //Pipe = 220, //Oem5 = 220,
        CloseBrackets = 221, //Oem6 = 221,
        Quotes = 222, //Oem7 = 222,
        //Oem8 = 223,
        Backslash = 226, //Oem102 = 226,

        //Break = 246,

        //Play = 250,

        Clear = 254,
    }
}
