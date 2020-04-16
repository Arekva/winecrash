using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Winecrash.Engine
{
    internal class LinuxInputWrapper : IInputWrapper
    {
        public OSPlatform CorrespondingOS { get; } = OSPlatform.Linux;

        public bool GetKey(Keys key)
        {
            throw new NotImplementedException();
        }
    }
}
