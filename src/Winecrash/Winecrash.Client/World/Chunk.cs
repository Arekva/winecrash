using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Client
{
    public class Chunk : Module
    {
        public short this[int x, int y, int z]
        {
            get { return Blocks[x, y, z]; }

            set { Blocks[x, y, z] = value; }
        }
        public const int ChunkTotalBlocks = Height * Width * Depth;

        public const int RandomTickSpeed = 3;

        public const int Height = 256;
        public const int Width = 16;
        public const int Depth = 16;

        internal short[,,] Blocks;

        public Ticket Ticket { get; internal set; }


        /*public Chunk(Ticket ticket, short[,,] blocksCacheIndexes)
        {
            this.Ticket = ticket;
            this._Blocks = blocksCacheIndexes;
        }*/

        protected override void Creation()
        {

        }

        public int Get1DIndex(int x, int y, int z)
        {
            return x + Width * y + Width * Height * z;
        }

        public void Tick()
        {
            for (int i = 0; i < ChunkTotalBlocks; i += ChunkTotalBlocks / 16)
            {
                for (int j = 0; j < RandomTickSpeed; j++)
                {
                    //_blocks[World.WorldRandom.Next(i, ChunkTotalBlocks)].Tick();
                }
            }
        }

        protected override void OnDelete()
        {
            this.Blocks = null;
            this.Ticket = null;

            base.OnDelete();
        }

        private string NoCompress()
        {
            return JsonConvert.SerializeObject(this.ToDictionnary(), Formatting.None);
        }

        public string ToJSON()
        {
            return this.NoCompress();
        }

        internal DictionnaryChunk ToDictionnary()
        {
            List<string> distinctIDs = new List<string>(ChunkTotalBlocks);
            int[] blocksRef = new int[ChunkTotalBlocks];

            int chunkIndex = 0;

            for (int z = 0; z < Chunk.Depth; z++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        string id = ItemCache.Get(Blocks[x, y, z]).Identifier;//_blocks[i].Identifier;

                        if (!distinctIDs.Contains(id))
                        {
                            distinctIDs.Add(id);
                        }

                        blocksRef[chunkIndex] = distinctIDs.IndexOf(id);
                        chunkIndex++;
                    }
                }
            }


            return new DictionnaryChunk(distinctIDs.ToArray(), blocksRef);
        }

    }
}
