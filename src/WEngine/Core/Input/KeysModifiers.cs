using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEngine.Core.Input
{
    [System.Flags]
    public enum KeysModifiers
    {
        None = 0,
        Shift = 65536,
        Alt = 262144,
        Control = 131072
    }
}
