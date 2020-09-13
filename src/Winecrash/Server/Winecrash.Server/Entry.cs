using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;
using Winecrash;

namespace Winecrash.Server
{
    public static class Entry
    {
        public static void Main(string[] args)
        {
            Save save = new Save("test", false);

            Console.WriteLine(save.Informations.Name);
            
            Console.ReadKey();
        }
    }
}
