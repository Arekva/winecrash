using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    [Flags]
    public enum TextAligns
    {
        None = 0,
        Left = 2,
        Right = 4,
        Up = 8,
        Down = 16,
        Horizontal = Up | Down,
        Vertical = Left | Right,
        Middle = Horizontal | Vertical
    }
}
