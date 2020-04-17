using System;

namespace Winecrash.Engine
{
    [Flags]
    public enum LogCallbackTypes : byte
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        Exception = 8
    }
}