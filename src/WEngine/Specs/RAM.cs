using System;
using System.Management;

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
            RAM ram = new RAM();
            
            var rams = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");

            ulong totalCapacity = 0UL;
            
            foreach (var item in rams.Get())
            {
                totalCapacity += (ulong)item["Capacity"];
            }

            ram.Capacity = totalCapacity;
            
            return ram;
        }
    }
}