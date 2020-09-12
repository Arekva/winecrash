using System;

namespace WEngine
{
    /// <summary>
    /// All the log callbacks possible. Used by <see cref="Logger"/>.
    /// </summary>
    [Flags]
    public enum LogCallbackTypes
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        Exception = 8
    }
}