using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    [Serializable]
    public class JSONChunk
    {
        public string[] Palette;

        public int[] Data;
    }
}
