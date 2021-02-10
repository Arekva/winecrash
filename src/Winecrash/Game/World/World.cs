using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WEngine;
using Winecrash.Entities;
using Winecrash.Net;

using LibNoise;
using LibNoise.Combiner;
using LibNoise.Filter;
using LibNoise.Modifier;
using LibNoise.Primitive;

namespace Winecrash
{
    public static class World
    {
        public const int MaxDimension = 32;

        public static List<Chunk>[] Chunks { get; private set; } = new List<Chunk>[MaxDimension];
        public static List<Vector3I> ReservedChunks { get; private set; } = new List<Vector3I>();
        internal static object ChunksLocker { get; } = new object();
        public static List<Dimension> Dimensions { get; } = new List<Dimension>(1);
        internal static object DimensionLocker { get; } = new object();

        public static int? Seed { get; set; } = "arekva".GetHashCode();

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

        public static Chunk[] GetChunks(string dimension)
        {
            int dimIndex = Dimensions.IndexOf(GetOrCreateDimension(dimension));

            return Chunks[Dimensions.IndexOf(GetOrCreateDimension(dimension))].ToArray();
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
                chunk = CreateChunk(coordinates, dimIndex, null);
            }

            return chunk;
        }

        private static ushort[] GenerateLandmass(Vector2I chunkCoords)
        {
            float globalScale = 1.0F;
            
            float baseLandmassScale = 0.005F; // lower => wider. yeah ik.
            float detailBias = 0.0F;

            float oceanLevel = 63;
            float landDeformity = 40;
            float oceanDeformity = 20;
            float landMaxHeight = oceanLevel + landDeformity;
            float oceanMaxDepth = oceanLevel - oceanDeformity;
            float mountainsDeformity = 10;
            float moutainsScale = 0.01F;

            float maxLevel = 63 + 40;
            
            // this is meant to be used as a 2D base for continents     apparently unused \/
            IModule3D baseLandmass = new ImprovedPerlin(World.Seed.Value, NoiseQuality.Best);
            var billow = new Billow
            {
                Primitive3D = baseLandmass,
                OctaveCount = 5.0F,
                Frequency = 0.9F,
            };
            IModule3D detailsLandmass = new ImprovedPerlin(World.Seed.Value * 123456, NoiseQuality.Best);
            detailsLandmass = new ScaleBias(detailsLandmass, .2F, detailBias);
            baseLandmass = new Add(billow, detailsLandmass);
            //LibNoise.Modifier.Exponent baseLandExp = new Exponent(baseLandmass, 1.0F);

            //var combiner = new LibNoise.Combiner.Add(baseLandmass1, baseLandmass1);
            float oceanCoverage = 0.5F;
            
            ushort airID = ItemCache.GetIndex("winecrash:air");
            ushort stoneID = ItemCache.GetIndex("winecrash:stone");
            
            ushort[] indices = new ushort[Chunk.Width * Chunk.Height * Chunk.Depth];
            
            Vector3F shift = Vector3F.Zero;
            
            Vector3F basePos = new Vector3F((chunkCoords.X * Chunk.Width) + shift.X, shift.Y, (chunkCoords.Y * Chunk.Depth) + shift.Z);

            for (int i = 0; i < Chunk.Width * Chunk.Height * Chunk.Depth; i++)
            {
                //Parallel.For(0, Chunk.Width * Chunk.Height * Chunk.Depth, i =>
                //{
                indices[i] = airID;

                // get, from index, the x,y,z coordinates of the block. Then move it from the basePos x scale.
                WMath.FlatTo3D(i, Chunk.Width, Chunk.Height, out int x, out int y, out int z);

                if (y > maxLevel) continue;
                
                Vector3F finalPos = new Vector3F((basePos.X + x) * baseLandmassScale, basePos.Y + y,
                    (basePos.Z + z) * baseLandmassScale) * globalScale;

                float landMassPct = baseLandmass.GetValue(finalPos.X, 0, finalPos.Z);
                // retreive the 2D base land value (as seen from top). If under land cap, it's ocean.
                bool isLand = landMassPct > oceanCoverage;

                float landPct = (float) Math.Pow(WMath.Remap(landMassPct, oceanCoverage, 1.0F, 0.0F, 1.0F), 2F);
                float waterPct = WMath.Remap(landMassPct, 0.0F, oceanCoverage, 0.0F, 1.0F);

                float landMassHeight = oceanLevel + (landPct * landDeformity);
                int finalLandMassHeight = (int) landMassHeight;

                float waterHeight = oceanMaxDepth + (waterPct * oceanDeformity);
                int finalWaterHeight = (int) waterHeight;

                if (isLand)
                {
                    if (y < finalLandMassHeight)
                    {
                        indices[i] = stoneID;
                    }
                }
                else if (y == oceanLevel - 1)
                {
                    //indices[i] = waterID;
                }
                else if (y < oceanLevel - 1)
                {
                    if (y > finalWaterHeight)
                    {
                        //indices[i] = waterID;
                    }
                    else
                    {
                        indices[i] = stoneID;
                    }
                }


                // debug: display this as grass / sand.


                //indices[i] = y == 63 ? (isLand ? grassID : sandID) : (y > 63 ? airID : debugID);//debug.GetValue(finalPos.X, finalPos.Y, finalPos.Z) < cap ? stoneID : airID;
                //});
            }

            return indices;
        }

        private static ushort[] PaintLandmass(Vector2I chunkCoords, ushort[] indices, out Vector3I[] surfaces)
        {
            ushort airID = ItemCache.GetIndex("winecrash:air");
            ushort stoneID = ItemCache.GetIndex("winecrash:stone");
            ushort grassID = ItemCache.GetIndex("winecrash:grass");
            ushort dirtID = ItemCache.GetIndex("winecrash:dirt");
            ushort sandID = ItemCache.GetIndex("winecrash:sand");
            ushort waterID = ItemCache.GetIndex("winecrash:water");
            ushort bedrockID = ItemCache.GetIndex("winecrash:bedrock");

            //ushort[] indices = new ushort[Chunk.Width * Chunk.Height * Chunk.Depth];
            
            Vector3F shift = Vector3F.Zero;
            
            Vector3F basePos = new Vector3F((chunkCoords.X * Chunk.Width) + shift.X, shift.Y, (chunkCoords.Y * Chunk.Depth) + shift.Z);
            
            ConcurrentBag<Vector3I> allSurfaces = new ConcurrentBag<Vector3I>();

            // for each x and z, for y from top to bottom:
            for (int i = 0; i < Chunk.Width*Chunk.Depth; i++)
            { 
            //Parallel.For(0, Chunk.Width /** Chunk.Height*/ * Chunk.Depth, i =>
            //{
                // get the x / z position
                WMath.FlatTo2D(i, Chunk.Width, out int x, out int z);

                int surfaceHeight = Chunk.Height - 1;
                bool heightFound = false;
                for (int y = Chunk.Height - 1; y > -1; y--)
                {
                    
                    
                    int idx = WMath.Flatten3D(x, y, z, Chunk.Width, Chunk.Height);


                    // if height already have been found, check back if
                    // there is a surface or not (3D terrain, like grass under arches or so)
                    if (heightFound)
                    {
                        if (indices[idx] == airID) // if air found, reset height 
                        {
                            surfaceHeight = y;
                            heightFound = false;
                        }
                    }
                    else
                    {
                        surfaceHeight = y;
                        if (indices[idx] != airID)
                        {
                            heightFound = true;
                            allSurfaces.Add(new Vector3I(x,y,z));
                        }
                    }
                    
                    // second pass: check the difference between surface and
                    // the current block height.
                    if (heightFound)
                    {
                        int deltaHeight = surfaceHeight - y;

                        bool ocean = surfaceHeight < 64;
                        
                        // surface => grass
                        if (deltaHeight == 0)
                        {
                            indices[idx] = ocean ? sandID : grassID;
                        }
                        // dirt, under the grass
                        else if (deltaHeight < 3)
                        {
                            indices[idx] = ocean ? sandID : dirtID;
                        }
                    }

                    if (y < 64 && indices[idx] == airID)
                    {
                        indices[idx] = waterID;
                    }

                    if (y < 3)
                    {
                        double chance = Winecrash.Random.NextDouble();

                        if (y == 2 && chance < 0.33) indices[idx] = bedrockID;
                        else if (y == 1 && chance < 0.66) indices[idx] = bedrockID;
                        else if (y == 0) indices[idx] = bedrockID;
                    }
                }
                
                //Vector3F finalPos = new Vector3F((basePos.X + x) * baseLandmassScale, basePos.Y + y,
                //    (basePos.Z + z) * baseLandmassScale) * globalScale;
            //});
            }

            surfaces = allSurfaces.ToArray();

            return indices;
        }

        private static ushort[] Populate(Vector2I chunkCoords, ushort[] indices, Vector3I[] surfaces)
        {
            double treeDensity = 0.005D;

            int chunkHash = chunkCoords.GetHashCode() * Seed.Value;
            
            Random treeRandom = new Random(chunkHash);
            Structure tree = Structure.Get("Tree");

            ushort dirtID = ItemCache.GetIndex("winecrash:dirt");
            ushort grassID = ItemCache.GetIndex("winecrash:grass");
            ushort debugID = ItemCache.GetIndex("winecrash:direction");
            
            // do trees
            for (int i = 0; i < surfaces.Length; i++)
            {
                int flatten = WMath.Flatten3D(surfaces[i].X, surfaces[i].Y, surfaces[i].Z, Chunk.Width, Chunk.Height);
                bool doTree = treeRandom.NextDouble() < treeDensity;
                
                if ((indices[flatten] == dirtID || indices[flatten] == grassID) && doTree)
                {
                    indices[flatten] = dirtID;
                    
                    PlaceStructure(tree, LocalToGlobal(chunkCoords, new Vector3D(surfaces[i].X, surfaces[i].Y + 1, surfaces[i].Z)), false);
                }
            }

            return indices;
        }
        private static ushort[] GenerateSimple(Vector2I chunkCoords, out Vector3I[] surfaces)
        {
            /*Dictionary<string, ushort> idsCache = new Dictionary<string, ushort>();
            
            ushort[] blocks = new ushort[Chunk.TotalBlocks];
            const string airID = "winecrash:air";*/


            ushort[] indices = GenerateLandmass(chunkCoords);
            
            PaintLandmass(chunkCoords, indices, out surfaces);

            return indices;
        }

        public static void PlaceStructure(Structure structure, Vector3I position, bool eraseSolid = false)
        {
            Vector3I shift = structure.Root;
            Vector3I origin = position - shift;
            Vector3I extents = structure.Size;
            
            ConcurrentDictionary<Vector2I, Chunk> chunkCache = new ConcurrentDictionary<Vector2I, Chunk>();

            ushort[] blocks = structure.Blocks;

            ushort airID = ItemCache.GetIndex("winecrash:air");

            Parallel.For(0, blocks.Length, i =>
            {
                WMath.FlatTo3D(i, extents.X, extents.Y, out int localX, out int localY, out int localZ);

                Vector3I globalBpos = new Vector3I(localX, localY, localZ) + origin;

                GlobalToLocal(globalBpos, out Vector2I cpos, out Vector3I bpos);

                Chunk c;
                if (!chunkCache.TryGetValue(cpos, out c))
                {
                    c = GetChunk(cpos, "winecrash:overworld");
                    chunkCache.TryAdd(cpos, c);
                }

                if (c && (eraseSolid || c.GetBlockIndex(bpos.X, bpos.Y, bpos.Z) == airID))
                {
                    c.SetBlock(bpos.X, bpos.Y, bpos.Z, blocks[i]);
                }
            });

            if (Engine.DoGUI) foreach (Chunk chunk in chunkCache.Values) if (chunk) chunk.BuildEndFrame = true;

            chunkCache.Clear();
            chunkCache = null;

        }

        public static void RebuildDimension(string dimension)
        {
            Chunk[] chunks = null;
            lock (ChunksLocker)
            {
                chunks = Chunks[Dimensions.IndexOf(GetOrCreateDimension(dimension))].ToArray();
            }

            Task.Run(() =>
            {
                Parallel.For(0, chunks.Length, i =>
                {
                    ushort[] blocks = GenerateSimple(chunks[i].Coordinates, out Vector3I[] surfaces);
                    chunks[i].Blocks = blocks;
                    Populate(chunks[i].Coordinates, blocks, surfaces);
                    
                    chunks[i].BuildEndFrame = true;
                });
            });
        }

        private static Chunk CreateChunk(Vector2I coordinates, int dimension, ushort[] blocks = null, bool build = true)
        {
            lock (ChunksLocker)
            {
                Vector3I coords = new Vector3I(coordinates, dimension);

                if (ReservedChunks.Contains(coords))
                    return null;
                else ReservedChunks.Add(coords);
            }
            try
            {
                Chunk chunk = null;
                Vector3I[] surfaces = null;

                bool generated = false;

                if (blocks == null)
                {
                    SaveChunk sc = Winecrash.CurrentSave.ReadChunk(coordinates, Dimensions.ElementAt(dimension).Identifier);
                    if (sc != null) blocks = sc.LoadBlocks();
                    else
                    {
                        generated = true;
                        blocks = GenerateSimple(coordinates, out surfaces);
                    }
                }

                WObject wobj = new WObject($"Chunk [{coordinates.X};{coordinates.Y}]")
                {
                    Parent = GetOrCreateDimensionWObject(Dimensions.ElementAt(dimension)),
                    LocalPosition = new Vector3D(coordinates.X * 16, 0, coordinates.Y * 16)
                };

                chunk = wobj.AddModule<Chunk>();
                chunk.InterdimensionalCoordinates = new Vector3I(coordinates, dimension);
                chunk.Blocks = blocks;

                if (generated)
                {
                    Task.Run(() => Winecrash.CurrentSave.WriteChunk(chunk.ToSave()));
                }

                Dimension dim = Dimensions.ElementAt(dimension);

                Vector2I region = coordinates / 8;

                string groupName = $"Region [{region.X};{region.Y} / {dim.Identifier}]";

                chunk.Group = groupName.GetHashCode();

                //ugh it doesn't work..?
                //Group.GetGroup(groupName.GetHashCode()).Name = groupName;


                lock (ChunksLocker)
                {
                    Chunks[dimension].Add(chunk);
                    ReservedChunks.Add(new Vector3I(coordinates, dimension));
                }
                
                if (surfaces != null)
                {
                    Populate(coordinates, blocks, surfaces);
                }

                if (Engine.DoGUI && build)
                {
                    chunk.BuildEndFrame = true;
                }
                
                return chunk;
            }
            catch (Exception e)
            {
                lock(ChunksLocker)
                    ReservedChunks.Remove(new Vector3I(coordinates, dimension));
                
                throw e;
            }
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

            Entity[] entities = null;
            lock (Entity.EntitiesLocker) entities = Entity.Entities.ToArray();

            for (int i = 0; i < entities.Length; i++) entities[i].Delete();
            Entity.Entities.Clear();


            Dimensions.Clear();

            WorldWObject?.Delete();
            WorldWObject = null;

            GC.Collect(2, GCCollectionMode.Forced, true, true);
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
            Debug.Log("Deleting " + c);
            c.WObject.Delete();
            
            lock(ChunksLocker)
                Chunks[Dimensions.IndexOf(c.Dimension)].Remove(c);
        }

        // Get the surface height at a certain position / dimension
        public static uint GetSurface(Vector3D position, string dimension)
        {
            GlobalToLocal(position, out Vector2I cpos, out Vector3I bpos);

            Chunk c = World.GetChunk(cpos, dimension);

            ushort[] blocks = null;

            if (c != null) blocks = c.Blocks;
            else blocks = GenerateSimple(cpos, out _);

            for (int i = 255; i > -1 ; i--)
            {
                if (!ItemCache.Get<Block>(blocks[WMath.Flatten3D(bpos.X, i, bpos.Z, Chunk.Width, Chunk.Height)]).Transparent)
                    return (uint)i;
            }

            return 0U;
        }
        public static void GlobalToLocal(Vector3D global, out Vector2I ChunkPosition, out Vector3I LocalPosition)
        {
            ChunkPosition = new Vector2I(
                ((int) global.X / Chunk.Width) + (global.X < 0.0F ? -1 : 0), //X position
                ((int) global.Z / Chunk.Depth) + (global.Z < 0.0F ? -1 : 0));//, //Y position
                //0);                                                         //Z dimension

            double localX = global.X % Chunk.Width;
            if (localX < 0) localX += Chunk.Width;

            double localZ = global.Z % Chunk.Depth;
            if (localZ < 0) localZ += Chunk.Depth;

            LocalPosition = new Vector3I((int)localX, (int)global.Y, (int)localZ);
        }

        public static Vector3I RelativePositionToChunk(Vector3I globalBlockPosition, Vector2I chunk)
        {
            Vector3I chunkPos = new Vector3I(chunk.X * Chunk.Width, 0, chunk.Y * Chunk.Depth);
            
            return new Vector3I(globalBlockPosition.X - chunkPos.X, globalBlockPosition.Y, globalBlockPosition.Z - chunkPos.Z);
        }
        public static Vector3I LocalToGlobal(Vector2I chunk, Vector3I block)
        {
            return new Vector3I(chunk.X * Chunk.Width, 0, chunk.Y * Chunk.Depth) + block;
        }

        public static RaycastChunkHit RaycastWorld(Ray ray)
        {
            GlobalToLocal(ray.Origin, out Vector2I minCPos, out _);
            GlobalToLocal(ray.Origin + ray.Direction * ray.Length, out Vector2I maxCPos, out _);
            
            var provider = new ChunkRayCollisionProvider();
            
            List<RaycastChunkHit> hits = new List<RaycastChunkHit>();
            
            // all the chunks to test raycast to
            for (int y = Math.Min(minCPos.Y, maxCPos.Y); y <= Math.Max(minCPos.Y, maxCPos.Y); y++)
            {
                for (int x = Math.Min(minCPos.X, maxCPos.X); x <= Math.Max(minCPos.X, maxCPos.X); x++)
                {
                    Vector2I pos = new Vector2I(x,y);
                    
                    Chunk c = GetChunk(pos, "winecrash:overworld");

                    if (c)
                    {
                        RaycastChunkHit hit = provider.Raycast(c, ray);
                        
                        if (hit.HasHit) hits.Add(hit);
                    }
                }
            }


            if (hits.Count != 0)
            {
                return hits.OrderBy(h => h.Distance).First();
            }
            
            return new RaycastChunkHit();
        }
    }
}