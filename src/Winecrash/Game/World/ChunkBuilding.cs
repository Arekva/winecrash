using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WEngine;
using Debug = WEngine.Debug;

namespace Winecrash
{
    public sealed partial class Chunk
    {
        private static Thread _BuildThread;
        private static bool _RunBuild = true;
        private static ManualResetEvent _BuildLocker = new ManualResetEvent(false);
        
        private static ConcurrentQueue<Chunk> _BuildQueue = new ConcurrentQueue<Chunk>();

        static Chunk()
        {
            WEngine.Engine.OnStop += () =>
            {
                _BuildQueue = null;
                _RunBuild = false;
                _BuildThread = null;
                _BuildLocker.Set();
            };

            _BuildThread = new Thread(BuildChunksLoop)
            {
                Name = "Chunk builder",
                Priority = ThreadPriority.Highest,
                IsBackground = true
            };
            
            _BuildThread.Start();
        }

        private static void BuildChunksLoop()
        {
            while (_RunBuild)
            {
                if (_BuildQueue.Count == 0)
                {
                    _BuildLocker.Reset();
                }
                
                _BuildLocker.WaitOne();
                if (_RunBuild) // double check if not done.
                {
                    if (_BuildQueue.TryDequeue(out Chunk chunk))
                    {
                        if (chunk != null && !chunk.Deleted)
                        {
                            chunk.ConstructInternal();
                            GC.Collect();
                        }
                    }
                    else // wait a little moment so everything calms down... I guess?
                    {
                        Task.Delay(1).Wait();
                    }
                }
            }
        }
        
        #region Mesh construct methods

        /// <summary>
        /// Calls the building thread to build when possible.
        /// </summary>
        public void Construct()
        {
            // do not double enqueue a chunk.
            if(!_BuildQueue.Contains(this))
                _BuildQueue.Enqueue(this);

            _BuildLocker.Set();
        }
        
        Dictionary<ushort, Block> constructblocks = new Dictionary<ushort, Block>();
        private void ConstructInternal()
        {
            if (this.Deleted) return;

            if (this.Blocks == null) 
            {
                BuildEndFrame = true;
                return;
            }
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            cwest = WestNeighbor?.Blocks != null;
            if (cwest)
            {
                bwest = new ushort[Chunk.TotalBlocks];
                WestNeighbor.Blocks.CopyTo(bwest,0);
            }
            ceast = (EastNeighbor?.Blocks != null);
            if (ceast)
            {
                beast = new ushort[Chunk.TotalBlocks];
                EastNeighbor.Blocks.CopyTo(beast,0);
            }
            cnorth = (NorthNeighbor?.Blocks != null);
            if (cnorth)
            {
                bnorth = new ushort[Chunk.TotalBlocks];
                NorthNeighbor.Blocks.CopyTo(bnorth,0);
            }
            csouth = (SouthNeighbor?.Blocks != null);
            if (csouth)
            {
                bsouth = new ushort[Chunk.TotalBlocks];
                SouthNeighbor.Blocks.CopyTo(bsouth,0);
            }

            List<Vector3F> svertices = new List<Vector3F>(1 << 18);
            List<Vector2F> suv = new List<Vector2F>(1 << 18);
            List<Vector3F> snormals = new List<Vector3F>(1 << 18);
            List<uint> striangles = new List<uint>(1 << 18);
            uint striangle = 0;
            
            List<Vector3F> tvertices = new List<Vector3F>(1 << 18);
            List<Vector2F> tuv = new List<Vector2F>(1 << 18);
            List<Vector3F> tnormals = new List<Vector3F>(1 << 18);
            List<uint> ttriangles = new List<uint>(1 << 18);
            uint ttriangle = 0;

            Stack<BlockFaces> faces = new Stack<BlockFaces>(6);
            
            Block block = null;
            ushort index = 0;

            if (this.Deleted) return;

            Vector3F thisWP = this.WObject.Position;
            
            for (int z = 0; z < Depth; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (this.Deleted) return;
                        
                        index = this.Blocks[x + Width * y + Width * Height * z];
                        
                        if (!constructblocks.TryGetValue(index, out block))
                        {
                            block = ItemCache.Get<Block>(index);
                            constructblocks.Add(index, block);
                        }

                        if (block.Identifier == "winecrash:air") continue; // ignore if air

                        bool transparent = block.Transparent;

                        if (block.DrawAllSides)
                        {
                            faces.Push(BlockFaces.Up); // up
                            faces.Push(BlockFaces.Down); // down
                            faces.Push(BlockFaces.West); // west
                            faces.Push(BlockFaces.East); // east
                            faces.Push(BlockFaces.North); // north
                            faces.Push(BlockFaces.South); // south
                        }
                        else
                        {
                            if (IsTransparent(block, x, y + 1, z, constructblocks)) faces.Push(BlockFaces.Up); // up
                            if (IsTransparent(block, x, y - 1, z, constructblocks)) faces.Push(BlockFaces.Down); // down
                            if (IsTransparent(block, x - 1, y, z, constructblocks)) faces.Push(BlockFaces.West); // west
                            if (IsTransparent(block, x + 1, y, z, constructblocks)) faces.Push(BlockFaces.East); // east
                            if (IsTransparent(block, x, y, z + 1, constructblocks)) faces.Push(BlockFaces.North); // north
                            if (IsTransparent(block, x, y, z - 1, constructblocks)) faces.Push(BlockFaces.South); // south
                        }

                        foreach (BlockFaces face in faces)
                        {
                            CreateVerticesCube(x, y, z, face, transparent ? tvertices : svertices);

                            /*int count = usedvertices.Count;
                            for (int i = count - 6; i < count; i++)
                            {
                                Vector3F worldPos = thisWP + usedvertices[i] - new Vector3F(572, 0, 459);

                                Vector3F final = worldPos.RotateAround(new Vector3F(0,-360, 0), new Quaternion(worldPos.X * 0.1,0,worldPos.Z * 0.1));

                                
                                final += new Vector3F(572, 0, 459);
                                usedvertices[i] = final - thisWP;
                            }*/

                            CreateUVsCube(face, transparent ? tuv : suv, index);

                            CreateNormalsCube(face, transparent ? tnormals : snormals);

                            if (transparent)
                            {
                                ttriangles.AddRange(new uint[6]
                                    {ttriangle, ttriangle + 1, ttriangle + 2, ttriangle + 3, ttriangle + 4, ttriangle + 5});

                                ttriangle += 6;
                            }
                            else
                            {
                                striangles.AddRange(new uint[6]
                                    {striangle, striangle + 1, striangle + 2, striangle + 3, striangle + 4, striangle + 5});

                                striangle += 6;
                            }
                        }

                        faces.Clear();
                    }
                }
            }

            
            sw.Stop();
            
            Debug.Log("Chunk build time: " + sw.Elapsed.TotalMilliseconds.ToString("F2") + "ms");

            if (svertices.Count != 0)
            {
                if (this.SolidRenderer.Mesh == null)
                {
                    this.SolidRenderer.Mesh = new Mesh("Chunk Mesh");
                }

                this.SolidRenderer.Mesh.Vertices = svertices.ToArray();
                this.SolidRenderer.Mesh.Triangles = striangles.ToArray();
                this.SolidRenderer.Mesh.UVs = suv.ToArray();
                this.SolidRenderer.Mesh.Normals = snormals.ToArray();
                this.SolidRenderer.Mesh.Tangents = new Vector4F[svertices.Count];

                this.SolidRenderer.Mesh.Apply(true);
            }
            else
            {
                this.SolidRenderer.Mesh?.Delete();
                this.SolidRenderer.Mesh = null;
            }
            if (tvertices.Count != 0)
            {
                if (this.TransparentRenderer.Mesh == null)
                {
                    this.TransparentRenderer.Mesh = new Mesh("Chunk Transparent Mesh");
                }

                this.TransparentRenderer.Mesh.Vertices = tvertices.ToArray();
                this.TransparentRenderer.Mesh.Triangles = ttriangles.ToArray();
                this.TransparentRenderer.Mesh.UVs = tuv.ToArray();
                this.TransparentRenderer.Mesh.Normals = tnormals.ToArray();
                this.TransparentRenderer.Mesh.Tangents = new Vector4F[tvertices.Count];

                this.TransparentRenderer.Mesh.Apply(true);
            }
            else
            {
                this.TransparentRenderer.Mesh?.Delete();
                this.TransparentRenderer.Mesh = null;
            }
            
            svertices = null;
            striangles = null;
            suv = null;
            snormals = null;
            
            tvertices = null;
            ttriangles = null;
            tuv = null;
            tnormals = null;
            
            cwest = ceast = cnorth = csouth = false;
            bsouth = null;
            bnorth = null;
            beast = null;
            bwest = null;

            ConstructedOnce = true;
        }
        


        private static Vector3F up = Vector3F.Up;
        private static Vector3F down = Vector3F.Down;
        private static Vector3F left = Vector3F.Left;
        private static Vector3F right = Vector3F.Right;
        private static Vector3F forward = Vector3F.Forward;
        private static Vector3F south = Vector3F.Backward;

        private const float AntiShitCoef = 0.999999F;
        private static void CreateUVsCube(BlockFaces face, List<Vector2F> uvs, int cubeIdx)
        {
            float faceIDX = 0;

            switch(face)
            {
                case BlockFaces.East:
                    faceIDX = 0;
                    break;

                case BlockFaces.West:
                    faceIDX = 1;
                    break;

                case BlockFaces.Up:
                    faceIDX = 2;
                    break;

                case BlockFaces.Down:
                    faceIDX = 3;
                    break;

                case BlockFaces.North:
                    faceIDX = 4;
                    break;

                case BlockFaces.South:
                    faceIDX = 5;
                    break;
            }

            float idx = (float)cubeIdx;

            float w = ItemCache.TextureSize / (float)ItemCache.Atlas.Width;
            float h = ItemCache.TextureSize / (float)ItemCache.Atlas.Height;
            const int incr = 1;

            switch (face)
            {
                case BlockFaces.Up:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F(faceIDX * w, idx * h) * AntiShitCoef, //0
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //incr
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //2
                            
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //3
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //4
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h) * AntiShitCoef, //5
                        });
                    }
                    break;

                case BlockFaces.Down:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, (idx + incr)* h) * AntiShitCoef, //top left
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //bottom left
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //top right

                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //top right
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //bottom left
                            new Vector2F((faceIDX ) * w, (idx)* h) * AntiShitCoef, //top left
                        });
                    }
                    break;

                case BlockFaces.North:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //5
                            new Vector2F(faceIDX * w, idx * h) * AntiShitCoef, //4
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h) * AntiShitCoef, //3

                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h) * AntiShitCoef, //2
                            new Vector2F(faceIDX * w, idx * h) * AntiShitCoef, //incr
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //0
                        });
                    }
                    break;

                case BlockFaces.South:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h) * AntiShitCoef, //0 topleft
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //2 bottom left
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //incr top right
                            
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, // 4 top right
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //3 bottom left
                            new Vector2F(faceIDX * w, idx * h) * AntiShitCoef, //5 bottom right
                        });
                    }
                    break;


                case BlockFaces.West:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //5
                            new Vector2F(faceIDX * w, idx * h) * AntiShitCoef, //4
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h) * AntiShitCoef, //3

                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h) * AntiShitCoef, //2
                            new Vector2F(faceIDX * w, idx * h) * AntiShitCoef, //incr
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //0
                        });
                    }
                    break;

                case BlockFaces.East:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h) * AntiShitCoef, //5
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //3
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //4
                            
                            new Vector2F(faceIDX * w, (idx + incr) * h) * AntiShitCoef, //incr
                            new Vector2F((faceIDX + incr) * w, idx * h) * AntiShitCoef, //2
                            new Vector2F(faceIDX * w, idx * h) * AntiShitCoef, //0
                        });
                    }
                    break;
            }
            
        }
        private static void CreateNormalsCube(BlockFaces face, List<Vector3F> normals)
        {
            switch (face)
            {
                case BlockFaces.Up:
                    {
                        normals.AddRange(new[]
                        {
                            up, up, up, up, up, up
                        });
                    }
                    break;

                case BlockFaces.Down:
                    {
                        normals.AddRange(new[]
                        {
                            down, down, down, down, down, down
                        });
                    }
                    break;

                case BlockFaces.West:
                    {
                        normals.AddRange(new[]
                        {
                            left, left, left, left, left, left
                        });
                    }
                    break;

                case BlockFaces.East:
                    {
                        normals.AddRange(new[]
                        {
                            right, right, right, right, right, right
                        });
                    }
                    break;

                case BlockFaces.North:
                    {
                        normals.AddRange(new[]
                        {
                            forward, forward, forward, forward, forward, forward
                        });
                    }
                    break;

                case BlockFaces.South:
                    {
                        normals.AddRange(new[]
                        {
                            south, south, south, south, south, south
                        });
                    }
                    break;
            }
        }

        /// <summary>
        /// Creates the vertices required to display a cube.
        /// </summary>
        /// <param name="x">The X position of the cube.</param>
        /// <param name="y">The Y position of the cube.</param>
        /// <param name="z">The Z position of the cube.</param>
        /// <param name="face">The orientation of the face.</param>
        /// <param name="vertices">The vertice list to add to.</param>
        private const float AntiJitteringCoef = 1.00001F;
        private static void CreateVerticesCube(int x, int y, int z, BlockFaces face, List<Vector3F> vertices)
        {
            const int incr = 1;
            switch (face)
            {
                case BlockFaces.Up:
                    {
                        vertices.AddRange(new[] {
                            new Vector3F(x, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x + incr, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y + incr, z + incr) * AntiJitteringCoef
                        });
                    }
                    break;

                case BlockFaces.Down:
                    {
                        vertices.AddRange(new[] {
                            new Vector3F(x, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x, y, z) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x, y, z) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z) * AntiJitteringCoef
                        });
                    }
                    break;

                case BlockFaces.West:
                    {
                        vertices.AddRange(new[] {
                            new Vector3F(x, y, z) * AntiJitteringCoef,
                            new Vector3F(x, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z + incr)  * AntiJitteringCoef
                        });
                    }
                    break;

                case BlockFaces.East:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(x + incr, y + incr, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x + incr, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z) * AntiJitteringCoef
                        });
                    }
                    break;

                case BlockFaces.North:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(x, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z + incr) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z + incr) * AntiJitteringCoef,
                            new Vector3F(x + incr, y + incr, z + incr) * AntiJitteringCoef
                        });
                    }
                    break;

                case BlockFaces.South:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(x + incr, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x, y + incr, z) * AntiJitteringCoef,
                            new Vector3F(x + incr, y, z) * AntiJitteringCoef,
                            new Vector3F(x, y, z) * AntiJitteringCoef
                        });
                    }
                    break;
            }
        }

        bool cwest ; ushort[] bwest;
        bool ceast ; ushort[] beast;
        bool cnorth; ushort[] bnorth;
        bool csouth; ushort[] bsouth;


        ushort transpid;
        Block transblock;
        private bool IsTransparent(Block currentBlock, int x, int y, int z, Dictionary<ushort, Block> cache)
        {
            //if outside world, yes
            if (y < 0 || y > 255) return true;

            if (x < 0) // check west neighbor
            {
                if (cwest)
                {
                    transpid = this.bwest[15 + Width * y + Width * Height * z];
                }
            }

            else if (x > 15) // check east neighbor
            {
                if (ceast)
                {
                    transpid = this.beast[0 + Width * y + Width * Height * z];
                }
            }

            else if (z > 15) //check south neighbor
            {
                if (cnorth)
                {
                    transpid = this.bnorth[x + Width * y + Width * Height * 0];
                }
            }

            else if (z < 0)
            {
                if(csouth)
                {
                    transpid = this.bsouth[x + Width * y + Width * Height * 15];
                }
            }

            else
            {
                transpid = this.GetBlockIndex(x, y, z);
            }

            if (!cache.TryGetValue(transpid, out transblock))
            {
                transblock = ItemCache.Get<Block>(transpid);
                   cache.Add(transpid, transblock);
            }


            if (currentBlock == transblock)
            {
                return transblock.Transparent && transblock.DrawInternalFaces;
            }
            else
            {
                return transblock.Transparent;
            }
        }
        #endregion
    }
}