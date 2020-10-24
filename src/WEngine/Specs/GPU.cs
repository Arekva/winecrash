using System.Management;
using System.Runtime.InteropServices;

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
            if (!Engine.DoGUI)
            {
                return new GPU() { Name = "No GUI mode" };
            }
            
            if(Engine.OS.Platform == OSPlatform.Windows)
            {
                GPU gpu = new GPU();
                
                var gpus = new ManagementObjectSearcher("select * from Win32_VideoController");
                
                foreach (var item in gpus.Get())
                {
                    gpu.Name = (string)item["Name"];
                }
                
                return gpu;
            
            }
            else
            {
                return new GPU()
                {
                    Name = "TODO"
                };
            }
        }
    }
}