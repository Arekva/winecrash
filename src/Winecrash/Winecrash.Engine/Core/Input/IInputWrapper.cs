using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Winecrash.Engine
{
    internal interface IInputWrapper
    {
        OSPlatform CorrespondingOS { get; }
        bool GetKey(Keys key);

        Vector2I GetMousePosition();

        void SetMousePosition(Vector2I position);
    }
}
