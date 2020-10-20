using System;
using System.Runtime.InteropServices;
using JCS;

namespace WEngine
{
    public struct OS
    {
        //System OperatingSystem
        
        public OSPlatform Platform { get; private set; }
        public Version Version { get; private set; }

        public string Name
        {
            get
            {
                return this.Version.Name;
            }
        }

        /// <summary>
        /// Number of bits of the architecture. Should be 32 or 64. If unknown it will return -1.
        /// </summary>
        public int Architecture { get; private set; }

        public static OS FromCurrentConfig()
        {
            OS os = new OS();

            os.Platform = GetCurrentPlatform();

            switch (OSVersionInfo.OSBits)
            {
                case OSVersionInfo.SoftwareArchitecture.Bit32:
                    os.Architecture = 32;
                    break;
                case OSVersionInfo.SoftwareArchitecture.Bit64:
                    os.Architecture = 64;
                    break;
                default:
                    os.Architecture = -1;
                    break;
            }

            System.Version sversion = OSVersionInfo.Version;

            os.Version = new Version((uint)sversion.Major, (uint)sversion.Minor, (uint)sversion.Build, OSVersionInfo.Name);


            return os;
        }

        private static OSPlatform GetCurrentPlatform()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }
            
            throw new Exception("Running on an unknown platform.");
        }
    }
}