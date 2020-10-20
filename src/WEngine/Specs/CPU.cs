using System.Management;

namespace WEngine
{
    public struct CPU
    {
        /// <summary>
        /// The name of the CPU.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// The amount of cores
        /// </summary>
        public uint Cores { get; private set; }
        
        /// <summary>
        /// Clock frequency in GHz.
        /// </summary>
        public double Frequency { get; private set; }

        public static CPU FromCurrentConfig(uint cpuID = 0)
        {
            CPU cpu = new CPU();
            
            var cpus = new ManagementObjectSearcher("select * from Win32_Processor");
            
            //foreach CPU
            foreach (var item in cpus.Get())
            {
                cpu.Name = (string) item["Name"];
                cpu.Frequency = 0.001D * (uint) item["MaxClockSpeed"];
                cpu.Cores = (uint) item["NumberOfCores"];
            }
            
            return cpu;
        }
    }
}