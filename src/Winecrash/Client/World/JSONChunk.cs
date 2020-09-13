using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Game
{
    [Serializable]
    public class JSONChunk
    {
        public bool Populated;

        public string[] Palette;

        public int[] Data;
    }
}
