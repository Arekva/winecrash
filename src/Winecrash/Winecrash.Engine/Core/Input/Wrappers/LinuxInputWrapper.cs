using System;
using System.Runtime.InteropServices;
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
