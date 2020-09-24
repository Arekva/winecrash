using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using WEngine;

namespace Winecrash
{
    public static class World
    {
        public const int MaxDimension = 32;

        public static List<Chunk>[] Chunks { get; private set; } = new List<Chunk>[MaxDimension];
        internal static object ChunksLocker { get; } = new object();
        public static List<Dimension> Dimensions { get; } = new List<Dimension>(1);

        public static WObject WorldWObject { get; set; }

        static World()
        {
            GetOrCreateDimension("winecrash:overworld");
        }

        public static Dimension GetOrCreateDimension(string identifier)
        {
            Dimension dim = Dimensions.FirstOrDefault(d => d.Identifier == identifier);
            
            if (dim != null)
            {
                return dim;
            }
            else
            {
                dim = new Dimension(identifier);
                Dimensions.Add(dim);
                
                Chunks[Dimensions.IndexOf(dim)] = new List<Chunk>(256);
                
                return dim;
            }
        }
        
        private static WObject GetOrCreateDimensionWObject(Dimension dimension)
        {
            if(WorldWObject == null) WorldWObject = new WObject("World");
            
            return WorldWObject.FindChild(dimension.Identifier) ??
                   new WObject(dimension.Identifier) {Parent = WorldWObject};
        }

        public static Chunk GetOrCreateChunk(SaveChunk saveChunk)
        {
            int dimIndex = Dimensions.IndexOf(GetOrCreateDimension(saveChunk.Dimension));

            Chunk[] dimChunks = null;
            lock (ChunksLocker)
            {
                dimChunks = Chunks[dimIndex].ToArray();
            }

            Chunk chunk = dimChunks.FirstOrDefault(c => c.Coordinates == saveChunk.Coordinates);

            if (!chunk)
            {
                return CreateChunk(saveChunk.Coordinates, dimIndex, saveChunk.LoadBlocks());
            }
            else
            {
                return chunk;
            }
        }
        
        public static Chunk GetOrCreateChunk(Vector2I coordinates, string dimension)
        {
            int dimIndex = Dimensions.IndexOf(GetOrCreateDimension(dimension));

            Chunk[] dimChunks = null;
            lock (ChunksLocker)
            {
                dimChunks = Chunks[dimIndex].ToArray();
            }

            Chunk chunk = dimChunks.FirstOrDefault(c => c.Coordinates == coordinates);

            if (!chunk)
            {
                return CreateChunk(coordinates, dimIndex, new ushort[Chunk.TotalBlocks]);
            }
            else
            {
                return chunk;
            }
        }
        
        private static Chunk CreateChunk(Vector2I coordinates, int dimension, ushort[] blocks)
        {
            Chunk chunk = null;

            WObject wobj = new WObject($"Chunk [{coordinates.X};{coordinates.Y}]")
            {
                Parent = GetOrCreateDimensionWObject(Dimensions.ElementAt(dimension)),
                LocalPosition = new Vector3D(coordinates.X * 16, 0, coordinates.Y * 16)
            };

            chunk = wobj.AddModule<Chunk>();
            chunk.InterdimensionalCoordinates = new Vector3I(coordinates, dimension);
            chunk.Blocks = blocks;
            
            return chunk;
        }
    }
}