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
        public OSPlatform CorrespondingOS { get; }
        public bool GetKey(Keys key);
    }
}
