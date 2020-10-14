using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Threading.Tasks;
using WEngine;
using Winecrash.Net;

namespace Winecrash
{
    public static class World
    {
        public const int MaxDimension = 32;

        public static List<Chunk>[] Chunks { get; private set; } = new List<Chunk>[MaxDimension];
        internal static object ChunksLocker { get; } = new object();
        public static List<Dimension> Dimensions { get; } = new List<Dimension>(1);
        internal static object DimensionLocker { get; } = new object();

        public static WObject WorldWObject { get; set; }

        static World()
        {
            GetOrCreateDimension("winecrash:overworld");
        }

        public static Dimension GetOrCreateDimension(string identifier)
        {
            Dimension[] dimensions = null;
            lock (DimensionLocker) dimensions = Dimensions.ToArray();
            
            Dimension dim = dimensions.FirstOrDefault(d => d.Identifier == identifier);
            
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

        public static Chunk GetChunk(Vector2I coordinates, string dimension)
        {
            int dimIndex = Dimensions.IndexOf(GetOrCreateDimension(dimension));

            Chunk[] dimChunks = null;
            lock (ChunksLocker)
            {
                dimChunks = Chunks[dimIndex].ToArray();
            }

            return dimChunks.FirstOrDefault(c => c.Coordinates == coordinates);
        }

        public static Chunk GetOrCreateChunk(NetChunk nchunk)
        {
            int dimIndex = Dimensions.IndexOf(GetOrCreateDimension(nchunk.Dimension));

            Chunk[] dimChunks = null;
            lock (ChunksLocker)
            {
                dimChunks = Chunks[dimIndex].ToArray();
            }

            Chunk chunk = dimChunks.FirstOrDefault(c => c.Coordinates == nchunk.Coordinates);

            if (!chunk)
            {
                return CreateChunk(nchunk.Coordinates, dimIndex, nchunk.Indices);
            }
            else
            {
                //todo: edit chunk.
                if(nchunk.GetHashCode() != chunk.GetHashCode())
                {
                    chunk.Blocks = nchunk.Indices;

                    if(Engine.DoGUI)
                    {
                        chunk.BuildEndFrame = true;
                    }
                }

                return chunk;
            }
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
            
            //dimChunks = null;

            if (!chunk)
            {
                //Debug.Log(dimChunks.Length);
                //Debug.Log("CREATING CHUNK");
                chunk = CreateChunk(coordinates, dimIndex, GenerateSimple());
            }

            return chunk;
        }

        private static ushort[] GenerateSimple()
        {
            Dictionary<string, ushort> idsCache = new Dictionary<string, ushort>();
            
            ushort[] blocks = new ushort[Chunk.TotalBlocks];
            const string airID = "winecrash:air";
            
            for (int z = 0; z < Chunk.Depth; z++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        string id = airID;

                        int idx = x + Chunk.Width * y + Chunk.Width * Chunk.Height * z;
                        
                        if (y < 64)
                        {
                            if (y == 63)
                            {
                                id = "winecrash:grass";
                            }
                            else if (y > 60)
                            {
                                id = "winecrash:dirt";
                            }
                            else if (y > 3)
                            {
                                id = "winecrash:stone";
                            }
                            else
                            {
                                id = "winecrash:bedrock";
                            }
                        }

                        if(!idsCache.TryGetValue(id, out ushort cacheindex))
                        {
                            cacheindex = ItemCache.GetIndex(id);
                            idsCache.Add(id, cacheindex);
                        }
                        
                        blocks[idx] = cacheindex;
                    }
                }
            }

            return blocks;
        }
        
        private static Chunk CreateChunk(Vector2I coordinates, int dimension, ushort[] blocks, bool build = true)
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


            chunk.Group = int.Parse(dimension.ToString() + Math.Abs(((coordinates.X) / 4) + Math.Abs(((coordinates.Y) / 4))));

            //chunk.RunAsync = true;


            lock (ChunksLocker)
            {
                Chunks[dimension].Add(chunk);
            }

            if(Engine.DoGUI && build)
            {
                chunk.BuildEndFrame = true;
            }
            
            return chunk;
        }

        public static Vector2I[] GetCoordsInRange(Vector2I startPosition, uint renderDistance)
        {
            int x = startPosition.X;
            int y = startPosition.Y;

            List<Vector2I> pos = new List<Vector2I>((int)((renderDistance + 1) * (renderDistance + 1)));
            
            pos.Add(startPosition);
            
            // for each level until max is reached
            for (int dist = 1; dist <= renderDistance; dist++)
            {
                // limits
                int minx = x - dist;
                int maxx = x + dist;
                int maxy = y + dist;
                int miny = y - dist;

                // top line
                for (int i = minx; i < maxx; i++)
                {
                    pos.Add(new Vector2I(i, maxy));
                }

                // right line
                for (int i = maxy; i > miny; i--)
                {
                    pos.Add(new Vector2I(maxx, i));
                }

                // bottom line
                for (int i = maxx; i > minx; i--)
                {
                    pos.Add(new Vector2I(i, miny));
                }

                // left line
                for (int i = miny; i < maxy; i++)
                {
                    pos.Add(new Vector2I(minx, i));
                }
            }

            return pos.ToArray();
        }

        public static void Unload()
        {
            Parallel.ForEach(Dimensions, dim => UnloadDimension(dim));
        }

        public static void UnloadDimension(Dimension dimension)
        {
            Chunk[] chunks = null;
            lock (ChunksLocker)
            {
                chunks = Chunks[Dimensions.IndexOf(dimension)].ToArray();
            }

            for (int i = 0; i < chunks.Length; i++)
            {
                UnloadChunk(chunks[i]);
            }
            
            Chunks[Dimensions.IndexOf(dimension)].Clear();
        }
        public static void UnloadChunk(Vector2I coordinates, Dimension dimension)
        {
            Chunk[] chunks = null;
            lock (ChunksLocker) chunks = Chunks[Dimensions.IndexOf(dimension)].ToArray();

            UnloadChunk(chunks.FirstOrDefault(c => c.Coordinates == coordinates));
        }
        public static void UnloadChunk(Chunk c)
        {
            if (!c) return;
            
            c.Delete();
            
            lock(ChunksLocker)
                Chunks[Dimensions.IndexOf(c.Dimension)].Remove(c);
        }
    }
}