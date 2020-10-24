using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace WEngine
{
    public struct RAM
    {
        /// <summary>
        /// The total capacity of computer's RAM (byte).
        /// </summary>
        public ulong Capacity { get; private set; }

        public static RAM FromCurrentConfig()
        {
            if (Engine.OS.Platform == OSPlatform.Windows)
            {
                RAM ram = new RAM();

                var rams = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");

                ulong totalCapacity = 0UL;

                foreach (var item in rams.Get())
                {
                    totalCapacity += (ulong) item["Capacity"];
                }

                ram.Capacity = totalCapacity;

                return ram;
            }
            if (Engine.OS.Platform == OSPlatform.OSX)
            {
                return new RAM();
            }
            else
            {
                return new RAM();
                /*Process p = new Process();

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "/bin/bash";
                p.StartInfo.Arguments = "-c 'cat /proc/meminfo'";

                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                
                string[] lines = output.Split('\n');

                string totMemParam = "MemTotal";
            
                string totMemLine = lines.First(l => l.Contains(totMemParam));
                string totMem = totMemLine.Substring((nameParam + "=\"").Length, (nameParam + "=\"").Length - 1);

                System.Version sversion = os.OperatingSystem.Version;
                os.Version = new Version((uint) sversion.Major, (uint) sversion.Minor, (uint) sversion.Build,
                    name);*/
            }
        }
    }
}