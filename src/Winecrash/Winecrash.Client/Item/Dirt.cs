using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Client
{
    public class Dirt : Cube
    {
        public const int GrassPropagationLevel = 4;
        protected override void WorldTick(Chunk chunk, Vector3I position)
        {
            
            if (position.Y != 255 && !chunk[position.X, position.Y + 1, position.Z].Transparent)// || chunk.GetLightLevel(position.X, position.Y + 1, position.Z) < GrassPropagationLevel)
            {
                return;
            }

            int gx;
            int gy;
            int gz;

            for (int z = -1; z < 2; z++)
            {
                gz = position.Z + z;

                for (int y = -1; y < 2; y++)
                {
                    gy = position.Y + y;

                    if (gy == 256 || gy == -1) continue;

                    for (int x = -1; x < 2; x++)
                    {
                        if (x == 0 && z == 0) continue;

                        gx = position.X + x;

                        if (World.GetBlock(World.LocalToGlobal(chunk.Position.XY, new Vector3I(gx,gy,gz)))?.Identifier == "winecrash:grass")
                        {
                            chunk.Edit(position.X, position.Y, position.Z, ItemCache.Get<Block>("winecrash:grass"));
                            return;
                        }
                    }
                }
            }
        }
    }
}
