using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    public enum TickType
    {
        /// <summary>
        /// World, random world tick, mostly used to update grass, crops...
        /// </summary>
        World,
        /// <summary>
        /// Block tick, used when a player updates a neighbor block
        /// </summary>
        Block
    }
}
