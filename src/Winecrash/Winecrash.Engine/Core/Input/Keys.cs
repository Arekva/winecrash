namespace WEngine
{
    /// <summary>
    /// The keys recognized by the engine. Also contains the mouse buttons (1 to 6).
    /// </summary>
    [System.Flags]
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
