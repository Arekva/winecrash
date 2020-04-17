using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Winecrash.Engine
{
    public static class Engine
    {
        public static OSPlatform OS { get; private set; }

        private static OSPlatform[] SupportedOS = new OSPlatform[] { OSPlatform.Windows, OSPlatform.Linux };

        public static void Load()
        {
            Initializer.InitializeEngine();
        }
        public static void Stop()
        {
            Updater.UpdateThread?.Abort();
            Updater.FixedUpdateThread?.Abort();
        }

        [Initializer(Int32.MinValue + 10)]
        private static void Initialize()
        {
            CheckPlateform();

            if(!SupportedOS.Contains(OS))
            {
                string errorMessage = "Sorry, but Winecrash is not compatible with system " + OS.ToString();

                try
                {
                    MessageBox.Show(errorMessage, "Fatal error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    throw new Exception(errorMessage);
                }
            }
            
            Debug.Log(OS);
        }

        private static void CheckPlateform()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OS = OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OS = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OS = OSPlatform.OSX;
            }
        }
    }
}
