using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Game
{
    public class Plant : Cube
    {
        protected override void WorldTick(Chunk chunk, Vector3I position)
        { 
            chunk.Edit(position.X, position.Y, position.Z, ItemCache.Get<Block>("winecrash:log"));
        }
    }
}
