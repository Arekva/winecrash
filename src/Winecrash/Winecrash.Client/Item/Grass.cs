using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Game
{
    public class Grass : Cube
    {
        protected override void WorldTick(Chunk chunk, Vector3I position)
        {
            
            if (position.Y == 255) return;
       
            else if(!chunk[position.X, position.Y + 1, position.Z].Transparent)
            {
                chunk.Edit(position.X, position.Y, position.Z, ItemCache.Get<Block>("winecrash:dirt"));
            }
        }
    }
}
