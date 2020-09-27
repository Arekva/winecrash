using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;

namespace Winecrash
{
    public class ChunkEventArgs : EventArgs
    {
        public Chunk Chunk { get; } = null;

        public ChunkEventArgs(Chunk chunk) : base()
        {
            this.Chunk = chunk;
        }
    }

    public delegate void ChunkDelegate(ChunkEventArgs args);
    
    public class Chunk : Module
    {

#region Constants
        public const int Width = 16;
        public const int Height = 256;
        public const int Depth = 16;
        public const int TotalBlocks = Width * Height * Depth;
#endregion

#region Block Getters/Setters
        public Block this[int x, int y, int z]
        {
            get
            {
                return this.GetBlock(x, y, z);
            }

            set
            {
                this.SetBlock(x, y, z, value);
            }
        }

        public void SetBlock(int x, int y, int z, Block b)
        {
            this.Blocks[x + Width * y + Width * Height * z] = ItemCache.GetIndex(b);
        }
        public void SetBlock(int x, int y, int z, string identifier)
        {
            this.Blocks[x + Width * y + Width * Height * z] = ItemCache.GetIndex(identifier);
        }
        public void SetBlock(int x, int y, int z, ushort cacheIndex)
        {
            this.Blocks[x + Width * y + Width * Height * z] = cacheIndex;
        }

        public string GetBlockIdentifier(int x, int y, int z)
        {
            return ItemCache.GetIdentifier(this.Blocks[x + Width * y + Width * Height * z]);
        }
        public ushort GetBlockIndex(int x, int y, int z)
        {
            return this.Blocks[x + Width * y + Width * Height * z];
        }
        
        public Block GetBlock(int x, int y, int z)
        {
            return ItemCache.Get<Block>(this.Blocks[x + Width * y + Width * Height * z]);
        }
#endregion

#region Fields and Properties
        /// <summary>
        /// The blocks references of this chunk.
        /// </summary>
        public ushort[] Blocks { get; set; }

        public Vector3I InterdimensionalCoordinates { get; set; }

        public Vector2I Coordinates
        {
            get
            {
                return this.InterdimensionalCoordinates.XY;
            }
            set
            {
                this.InterdimensionalCoordinates = new Vector3I(value.XY, this.InterdimensionalCoordinates.Z);
            }
        }

        public Dimension Dimension
        {
            get
            {
                return World.Dimensions[this.InterdimensionalCoordinates.Z];
            }
            set
            {
                this.InterdimensionalCoordinates = new Vector3I(this.InterdimensionalCoordinates.XZ, World.Dimensions.IndexOf(value));
            }
        }
        
        public static Texture Texture { get; set; }
#endregion

#region Neighbors
        /// <summary>
        /// Northern neighbor chunk
        /// </summary>
        public Chunk NorthNeighbor { get; set; } = null;
        /// <summary>
        /// Southern neighbor chunk
        /// </summary>
        public Chunk SouthNeighbor { get; set; } = null;
        /// <summary>
        /// Eastern neighbor chunk
        /// </summary>
        public Chunk EastNeighbor { get; set; } = null;
        /// <summary>
        /// Western neighbor chunk
        /// </summary>
        public Chunk WestNeighbor { get; set; } = null;
#endregion

#region Events
        public static event ChunkDelegate OnChunkSpawn;
#endregion
        public SaveChunk ToSave()
        {
            Dictionary<string, ushort> distinctIDs = new Dictionary<string, ushort>(64);
            ushort[] blocksRef = new ushort[Chunk.TotalBlocks];
            
            int chunkIndex = 0;
            ushort paletteIndex = 0;
            
            for (int z = 0; z < Chunk.Depth; z++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        string id = this[x, y, z].Identifier;

                        if (!distinctIDs.ContainsKey(id))
                        {
                            distinctIDs.Add(id, paletteIndex++);
                        }

                        blocksRef[chunkIndex++] = distinctIDs[id];
                    }
                }
            }

            SaveChunk sc = new SaveChunk()
            {
                Coordinates = Coordinates,
                Dimension = Dimension.Identifier,
                Indices = blocksRef,
                Palette = distinctIDs.Keys.ToArray()
            };

            return sc;
        }
        
        

#region Engine Logic
        protected override void OnDelete()
        {
            this.NorthNeighbor = null;
            this.SouthNeighbor = null;
            this.WestNeighbor = null;
            this.EastNeighbor = null;
            this.Blocks = null;
            base.OnDelete();
        }
#endregion
    }
}
