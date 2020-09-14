using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Game
{
    public struct TickWaitItem
    {
        public TickType Type { get; }
        public Chunk Chunk { get; }
        public Vector3I Position { get; }
        public Block Block { get; }

        public TickWaitItem(TickType type, Chunk chunk, Vector3I position, Block block)
        {
            Type = type;
            Chunk = chunk;
            Position = position;
            Block = block;
        }
    }
}
