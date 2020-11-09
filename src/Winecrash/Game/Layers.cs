using System;

namespace Winecrash
{
    public enum Layers : ulong
    {
        Nothing = 0UL,
        Default = 1UL,
        Sky = 1UL << 32,
        UI = 1UL << 48,
        Everything = UInt64.MaxValue,
        
        
    }
}