using WEngine;

namespace Winecrash
{
    public class SaveChunk
    {
        public Vector2I Coordinates;
        public string Dimension;
        public string[] Palette;
        public ushort[] Indices;

        public ushort[] LoadBlocks()
        {
            ushort[] blocks = new ushort[Chunk.Width * Chunk.Height * Chunk.Depth];
            int chunkindex = 0;
            
            for (int z = 0; z < Chunk.Depth; z++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        blocks[x + Chunk.Width * y + Chunk.Width * Chunk.Height * z] = ItemCache.GetIndex(Palette[Indices[chunkindex++]]);
                    }
                }
            }

            return blocks;
        }
        
        
    }
}