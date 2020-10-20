using System.Management;

namespace WEngine
{
    public struct GPU
    {
        /// <summary>
        /// The name of the CPU.
        /// </summary>
        public string Name { get; private set; }

        public static GPU FromCurrentConfig(uint gpuID = 0)
        {
            GPU gpu = new GPU();
            
            var gpus = new ManagementObjectSearcher("select * from Win32_VideoController");
            
            foreach (var item in gpus.Get())
            {
                gpu.Name = (string)item["Name"];
            }
            
            return gpu;
        }
    }
}