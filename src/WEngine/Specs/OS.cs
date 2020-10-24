using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using JCS;

namespace WEngine
{
    public struct OS
    {
        public OSPlatform Platform { get; private set; }
        public Version Version { get; private set; }
        public OperatingSystem OperatingSystem { get; set; }

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
            os.OperatingSystem = Environment.OSVersion;

            if (os.Platform == OSPlatform.Windows)
            {
                GetOSInfosWindows(ref os);
            }
            else if (os.Platform == OSPlatform.OSX)
            {
                GetOSInfosOSX(ref os);
            }
            else
            {
                GetOSInfosLinux(ref os);
            }

            return os;
        }
        
        private static void GetOSInfosLinux(ref OS os)
        {
            Process p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "/bin/bash";
            p.StartInfo.Arguments = "-c 'cat /etc/*-release'";

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            os.Architecture = IntPtr.Size * 8;
            
            string[] lines = output.Split('\n');

            string nameParam = "PRETTY_NAME";
            
            string nameLine = lines.First(l => l.Contains(nameParam));
            string name = nameLine.Substring((nameParam + "=\"").Length, (nameParam + "=\"").Length - 1);

            System.Version sversion = os.OperatingSystem.Version;
            os.Version = new Version((uint) sversion.Major, (uint) sversion.Minor, (uint) sversion.Build,
                name);
        }

        private static void GetOSInfosOSX(ref OS os)
        {
            os.Architecture = IntPtr.Size * 8;
            System.Version sversion = os.OperatingSystem.Version;
            os.Version = new Version((uint) sversion.Major, (uint) sversion.Minor, (uint) sversion.Build,
                os.Platform.ToString());
        }

        private static void GetOSInfosWindows(ref OS os)
        {
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

            os.Version = new Version((uint) sversion.Major, (uint) sversion.Minor, (uint) sversion.Build,
                OSVersionInfo.Name);
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