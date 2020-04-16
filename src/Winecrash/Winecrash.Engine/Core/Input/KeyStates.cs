using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    [Flags]
    internal enum KeyStates : byte
    {
        None = 0,
        Released = 1,
        Pressed = 2,
        Releasing = 4,
        Pressing = 8
    }
}
