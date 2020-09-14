using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Winecrash.Game
{
    public struct RaycastChunkHit
    {
        public Block Block { get; }
        public Chunk Chunk { get; }

        public Vector3I LocalPosition { get; }

        public Vector3F GlobalPosition { get; }

        public Vector3F Normal { get; }

        public double Distance { get; }

        

        public RaycastChunkHit(Vector3F position, Vector3F normal, double distance, Block block, Chunk chunk, Vector3I localPosition)
        {
            this.Block = block;
            this.Chunk = chunk;
            this.LocalPosition = localPosition;
            this.GlobalPosition = position;
            this.Normal = normal;
            this.Distance = distance;
        }
    }
}
