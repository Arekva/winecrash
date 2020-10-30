using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using Debug = WEngine.Debug;

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
    
    public class Chunk : Module, ICollider
    {

        static Chunk()
        {
            
        }

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
        
        public MeshRenderer BlocksRenderer { get; private set; }

        public static Texture Texture { get; set; }

        public bool BuildEndFrame { get; set; } = false;

        public bool ConstructedOnce { get; private set; } = false;
#endregion

#region Events
        public static event ChunkDelegate OnChunkLoad;
        public static event ChunkDelegate OnChunkUnload;
#endregion

#region Neighbors
        /// <summary>
        /// Northern neighbor chunk
        /// </summary>
        private Chunk _NorthNeighbor = null;

        public Chunk NorthNeighbor
        {
            get
            {
                if (this.Deleted) return null;
                if (_NorthNeighbor == null)
                {
                    _NorthNeighbor = World.GetChunk(this.Coordinates + Vector2I.Up, this.Dimension.Identifier);
                    if (_NorthNeighbor && _NorthNeighbor.SouthNeighbor == null)
                    {
                        _NorthNeighbor.SouthNeighbor = this;
                        _NorthNeighbor.BuildEndFrame = true;
                    }
                }

                return _NorthNeighbor;
            }

            set
            {
                if (this.Deleted) return;
                this._NorthNeighbor = value;
            }
        }
        /// <summary>
        /// Southern neighbor chunk
        /// </summary>
        private Chunk _SouthNeighbor = null;

        public Chunk SouthNeighbor
        {
            get
            {
                if (this.Deleted) return null;
                if (_SouthNeighbor == null)
                {
                    _SouthNeighbor = World.GetChunk(this.Coordinates + Vector2I.Down, this.Dimension.Identifier);
                    if (_SouthNeighbor && _SouthNeighbor.NorthNeighbor == null)
                    {
                        _SouthNeighbor.NorthNeighbor = this;
                        _SouthNeighbor.BuildEndFrame = true;
                    }
                }

                return _SouthNeighbor;
            }

            set
            {
                if (this.Deleted) return;
                this._SouthNeighbor = value;
            }
        }
        /// <summary>
        /// Eastern neighbor chunk
        /// </summary>
        private Chunk _EastNeighbor = null;

        public Chunk EastNeighbor
        {
            get
            {
                if (this.Deleted) return null;
                if (_EastNeighbor == null)
                {
                    _EastNeighbor = World.GetChunk(this.Coordinates + Vector2I.Right, this.Dimension.Identifier);
                    if (_EastNeighbor && _EastNeighbor.WestNeighbor == null)
                    {
                        _EastNeighbor.WestNeighbor = this;
                        _EastNeighbor.BuildEndFrame = true;
                    }
                }

                return _EastNeighbor;
            }

            set
            {
                if (this.Deleted) return;
                this._EastNeighbor = value;
            }
        }

        /// <summary>
        /// Western neighbor chunk
        /// </summary>
        private Chunk _WestNeighbor = null;
        public Chunk WestNeighbor
        {
            get
            {
                if (this.Deleted) return null;
                if (_WestNeighbor == null)
                {
                    _WestNeighbor = World.GetChunk(this.Coordinates + Vector2I.Left, this.Dimension.Identifier);
                    if (_WestNeighbor && _WestNeighbor.EastNeighbor == null)
                    {
                        _WestNeighbor.WestNeighbor = this;
                        _WestNeighbor.BuildEndFrame = true;
                    }
                }

                return _WestNeighbor;
            }

            set
            {
                if (this.Deleted) return;
                this._WestNeighbor = value;
            }
        }
#endregion
        public SaveChunk ToSave()
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //ConcurrentDictionary<string, ushort> distinctIDsC = new ConcurrentDictionary<string, ushort>();
            Dictionary<string, ushort> distinctIDs = new Dictionary<string, ushort>(64);
            ushort[] blocksRef = new ushort[Chunk.TotalBlocks];
            
            int chunkIndex = 0;
            ushort paletteIndex = 0;

            /*Parallel.For(0, Chunk.TotalBlocks, i =>
            {
                string id = ItemCache.GetIdentifier(this.Blocks[i]);
                
                //if (!distinctIDs.ContainsKey(id))
                //{
                if(!distinctIDsC.ContainsKey(id))
                    distinctIDsC.TryAdd(id, (ushort)(distinctIDsC.Count));
                //}

                blocksRef[i] = distinctIDsC[id];
                //Debug.Log(blocksRef[i]);
            });*/
            
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
            //sw.Stop();
            //Debug.Log("Save making time: " + sw.Elapsed.TotalMilliseconds.ToString("F0") + " ms");
            //Debug.Log();

            SaveChunk sc = new SaveChunk()
            {
                Coordinates = Coordinates,
                Dimension = Dimension.Identifier,
                Indices = blocksRef,
                Palette = distinctIDs.Keys.ToArray()
            };
            
            

            return sc;
        }

        
        private static object SafeLogicLocker = new object();
        private void OnChunkLoadedDelegate(ChunkEventArgs args)
        {
            return;
                Chunk c = args.Chunk;

                if (c == this) return;

                if (c.Dimension.Equals(this.Dimension))
                {
                    if (!NorthNeighbor && c.Coordinates == this.Coordinates.XY + Vector2I.Up)
                    {
                        this.BuildEndFrame = true;
                        c.BuildEndFrame = true;
                        NorthNeighbor = c;
                        c.SouthNeighbor = this;
                    }
                    else if (!SouthNeighbor && c.Coordinates == this.Coordinates.XY + Vector2I.Down)
                    {
                        this.BuildEndFrame = true;
                        c.BuildEndFrame = true;
                        SouthNeighbor = c;
                        c.NorthNeighbor = this;
                    }
                    else if (!WestNeighbor && c.Coordinates == this.Coordinates.XY + Vector2I.Left)
                    {
                        this.BuildEndFrame = true;
                        c.BuildEndFrame = true;
                        WestNeighbor = c;
                        c.EastNeighbor = this;
                    }
                    else if (!EastNeighbor && c.Coordinates == this.Coordinates.XY + Vector2I.Right)
                    {
                        this.BuildEndFrame = true;
                        c.BuildEndFrame = true;
                        EastNeighbor = c;
                        c.WestNeighbor = this;
                    }
                }
            
        }
        private void OnChunkUnloadDelegate(ChunkEventArgs args)
        {
            Chunk c = args.Chunk;
            if (c.Dimension.Equals(this.Dimension))
            {
                if (c == NorthNeighbor) NorthNeighbor = null;
                else if (c == SouthNeighbor) SouthNeighbor = null;
                else if (c == WestNeighbor) WestNeighbor = null;
                else if (c == EastNeighbor) EastNeighbor = null;
            }
        }

#region Engine Logic

        protected override void Creation()
        {
            OnChunkLoad += OnChunkLoadedDelegate;
            //lock (SafeLogicLocker)
            //{
                OnChunkLoad?.Invoke(new ChunkEventArgs(this));
            //}
            
            
            OnChunkUnload += OnChunkUnloadDelegate;

            this.RunAsync = true;

            if (Engine.DoGUI)
            {
                CreateRenderer();
            }
            
            
        }

        protected override void Start()
        {
            base.Start();
            
            if (Engine.DoGUI)
            {
                if(NorthNeighbor != null) NorthNeighbor.BuildEndFrame = true;
                if(SouthNeighbor != null) SouthNeighbor.BuildEndFrame = true;
                if(WestNeighbor != null) WestNeighbor.BuildEndFrame = true;
                if(EastNeighbor != null) EastNeighbor.BuildEndFrame = true;
            }
        }

        private void CreateRenderer()
        {
            BlocksRenderer = this.WObject.AddModule<MeshRenderer>();

            Material m = BlocksRenderer.Material = new Material(Shader.Find("Chunk"));
            m.SetData("albedo", Chunk.Texture);
            m.SetData("color", new Color256(1, 1, 1, 0));
            m.SetData("minLight", 0.1F);
            m.SetData("maxLight", 1.0F);
        }

        private double _TimeSinceAnimate = 0.0D;
        private double _AnimationTime = 0.5D;
        protected override void Update()
        {
            if (Engine.DoGUI)
            {
                UpdateMaterial();
            }
        }

        private void UpdateMaterial()
        {
            if (ConstructedOnce && _TimeSinceAnimate <= _AnimationTime)
            {
                _TimeSinceAnimate += Time.DeltaTime;
                BlocksRenderer.Material.SetData("color",
                    new Color256(1, 1, 1, WMath.Clamp(_TimeSinceAnimate / _AnimationTime, 0.0D, 1.0D)));
            }
        }

        protected override void LateUpdate()
        {
            if (Engine.DoGUI)
            {
                if (BuildEndFrame)
                {
                    BuildEndFrame = false;

                    Task.Run(Construct);
                }
            }

            base.LateUpdate();
        }

        protected override void OnDelete()
        {
            OnChunkLoad -= OnChunkLoadedDelegate;
            OnChunkUnload -= OnChunkUnloadDelegate;
            
            OnChunkUnload?.Invoke(new ChunkEventArgs(this));
            
            this.NorthNeighbor = null;
            this.SouthNeighbor = null;
            this.WestNeighbor = null;
            this.EastNeighbor = null;
            this.Blocks = null;

            if (Engine.DoGUI && this.BlocksRenderer)
            {
                this.BlocksRenderer.Mesh?.Delete();
                this.BlocksRenderer.Material?.Delete();
                this.BlocksRenderer.Delete();
                this.BlocksRenderer = null;
            }
            
            base.OnDelete();
        }
#endregion

#region oh boi

        public void Edit(int x, int y, int z, Block b = null)
        {
            PrivateEdit(x,y,z,b);

        /*#if !DEBUG
                    this.Ticket.Save();
        #endif*/

        }

        private void PrivateEdit(int x, int y, int z, Block b = null)
        {
            if (b == null) b = ItemCache.Get<Block>("winecrash:air");

            this[x, y, z] = b;

            if (x == 0 && this.WestNeighbor)
            {
                this.WestNeighbor.BuildEndFrame = true;
            }
            else if (x == 15 && this.EastNeighbor)
            {
                this.EastNeighbor.BuildEndFrame = true;
            }

            if (z == 0 && this.SouthNeighbor)
            {
                this.SouthNeighbor.BuildEndFrame = true;
            }
            else if (z == 15 && this.NorthNeighbor)
            {
                this.NorthNeighbor.BuildEndFrame = true;
            }

            BuildEndFrame = true;
        }

        internal static int ConstructingCounter = 0;
        internal static object ConstructLogicLocker = new object();
        public static int MaxConstructingAtOnce = 10;
        public void Construct()
        {
            if (this.Deleted) return;

            if (this.Blocks == null) 
            {
                BuildEndFrame = true;
                return;
            }

            lock (ConstructLogicLocker)
            {
                if(ConstructingCounter < MaxConstructingAtOnce) ConstructingCounter++;
                else
                {
                    BuildEndFrame = true;
                    return;
                }
            }

            cwest = this.WestNeighbor != null;
            ceast = this.EastNeighbor != null;
            cnorth = this.NorthNeighbor != null;
            csouth = this.SouthNeighbor != null;

            List<Vector3F> vertices = new List<Vector3F>();
            List<Vector2F> uv = new List<Vector2F>();
            List<Vector3F> normals = new List<Vector3F>();
            List<uint> triangles = new List<uint>();

            Stack<BlockFaces> faces = new Stack<BlockFaces>(6);
            uint triangle = 0;
            Block block = null;
            ushort index = 0;

            Dictionary<ushort, Block> blocks = new Dictionary<ushort, Block>();

           // Stopwatch buildTime = new Stopwatch();
            //buildTime.Start();
            for (int z = 0; z < Depth; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        index = this.Blocks[x + Width * y + Width * Height * z];
                        
                        if (!blocks.TryGetValue(index, out block))
                        {
                            block = ItemCache.Get<Block>(index);
                            blocks.Add(index, block);
                        }

                        if (block.Identifier == "winecrash:air") continue; // ignore if air

                        if (IsTransparent(x, y + 1, z, blocks)) faces.Push(BlockFaces.Up); // up
                        if (IsTransparent(x, y - 1, z, blocks)) faces.Push(BlockFaces.Down); // down
                        if (IsTransparent(x - 1, y, z, blocks)) faces.Push(BlockFaces.West); // west
                        if (IsTransparent(x + 1, y, z, blocks)) faces.Push(BlockFaces.East); // east
                        if (IsTransparent(x, y, z + 1, blocks)) faces.Push(BlockFaces.North); // north
                        if (IsTransparent(x, y, z - 1, blocks)) faces.Push(BlockFaces.South); // south

                        foreach (BlockFaces face in faces)
                        {
                            CreateVerticesCube(x, y, z, face, vertices);

                            CreateUVsCube(face, uv, index);

                            CreateNormalsCube(face, normals);

                            triangles.AddRange(new uint[6]
                                {triangle, triangle + 1, triangle + 2, triangle + 3, triangle + 4, triangle + 5});
                            triangle += 6;
                        }

                        faces.Clear();
                    }
                }
                

                
            }
            
            //buildTime.Stop();
            //Debug.Log("Chunk face build time : " + buildTime.Elapsed.TotalMilliseconds.ToString("F2") + "ms");

            //Stopwatch meshApply = new Stopwatch();
            //meshApply.Start();
            if (vertices.Count != 0)
            {
                //Debug.Log("vertices");
                if (this.BlocksRenderer.Mesh == null)
                {
                    this.BlocksRenderer.Mesh = new Mesh("Chunk Mesh");
                    //Engine.Debug.Log("Created mesh for " + this.Position);
                }

                this.BlocksRenderer.Mesh.Vertices = vertices.ToArray();
                this.BlocksRenderer.Mesh.Triangles = triangles.ToArray();
                this.BlocksRenderer.Mesh.UVs = uv.ToArray();
                this.BlocksRenderer.Mesh.Normals = normals.ToArray();
                this.BlocksRenderer.Mesh.Tangents = new Vector4F[vertices.Count];

                this.BlocksRenderer.Mesh.Apply(true);

                vertices = null;
                triangles = null;
                uv = null;
                normals = null;
            }
            
            //meshApply.Stop();
            //Debug.Log("Mesh apply time : " + meshApply.Elapsed.TotalMilliseconds.ToString("F2") + "ms");

            cwest = ceast = cnorth = csouth = false;

            ConstructedOnce = true;


            lock (ConstructLogicLocker)
            {
                ConstructingCounter--;
            }
        }


        private static Vector3F up = Vector3F.Up;
        private static Vector3F down = Vector3F.Down;
        private static Vector3F left = Vector3F.Left;
        private static Vector3F right = Vector3F.Right;
        private static Vector3F forward = Vector3F.Forward;
        private static Vector3F south = Vector3F.Backward;

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

            float w = ItemCache.TextureSize / (float)Texture.Width;
            float h = ItemCache.TextureSize / (float)Texture.Height;
            const int incr = 1;

            switch (face)
            {
                case BlockFaces.Up:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F(faceIDX * w, idx * h), //0
                            new Vector2F(faceIDX * w, (idx + incr) * h), //incr
                            new Vector2F((faceIDX + incr) * w, idx * h), //2
                            
                            new Vector2F((faceIDX + incr) * w, idx * h), //3
                            new Vector2F(faceIDX * w, (idx + incr) * h), //4
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h), //5
                        });
                    }
                    break;

                case BlockFaces.Down:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, (idx + incr)* h), //top left
                            new Vector2F((faceIDX + incr) * w, idx * h), //bottom left
                            new Vector2F(faceIDX * w, (idx + incr) * h), //top right

                            new Vector2F(faceIDX * w, (idx + incr) * h), //top right
                            new Vector2F((faceIDX + incr) * w, idx * h), //bottom left
                            new Vector2F((faceIDX ) * w, (idx)* h), //top left
                        });
                    }
                    break;

                case BlockFaces.North:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, idx * h), //5
                            new Vector2F(faceIDX * w, idx * h), //4
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h), //3

                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h), //2
                            new Vector2F(faceIDX * w, idx * h), //incr
                            new Vector2F(faceIDX * w, (idx + incr) * h), //0
                        });
                    }
                    break;

                case BlockFaces.South:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h), //0 topleft
                            new Vector2F((faceIDX + incr) * w, idx * h), //2 bottom left
                            new Vector2F(faceIDX * w, (idx + incr) * h), //incr top right
                            
                            new Vector2F(faceIDX * w, (idx + incr) * h), // 4 top right
                            new Vector2F((faceIDX + incr) * w, idx * h), //3 bottom left
                            new Vector2F(faceIDX * w, idx * h), //5 bottom right
                        });
                    }
                    break;


                case BlockFaces.West:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, idx * h), //5
                            new Vector2F(faceIDX * w, idx * h), //4
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h), //3

                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h), //2
                            new Vector2F(faceIDX * w, idx * h), //incr
                            new Vector2F(faceIDX * w, (idx + incr) * h), //0
                        });
                    }
                    break;

                case BlockFaces.East:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F((faceIDX + incr) * w, (idx + incr) * h), //5
                            new Vector2F((faceIDX + incr) * w, idx * h), //3
                            new Vector2F(faceIDX * w, (idx + incr) * h), //4
                            
                            new Vector2F(faceIDX * w, (idx + incr) * h), //incr
                            new Vector2F((faceIDX + incr) * w, idx * h), //2
                            new Vector2F(faceIDX * w, idx * h), //0
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

        bool cwest;
        bool ceast;
        bool cnorth;
        bool csouth;


        ushort transpid;
        Block transblock;
        private bool IsTransparent(int x, int y, int z, Dictionary<ushort, Block> cache)
        {
            //if outside world, yes
            if (y < 0 || y > 255) return true;

            if (x < 0) // check west neighbor
            {
                if (cwest)
                {
                    transpid = this.WestNeighbor.GetBlockIndex(15, y, z);
                }
            }

            else if (x > 15) // check east neighbor
            {
                if (ceast)
                {
                    transpid = this.EastNeighbor.GetBlockIndex(0, y, z);
                }
            }

            else if (z > 15) //check south neighbor
            {
                if (cnorth)
                {
                    transpid = this.NorthNeighbor.GetBlockIndex(x, y, 0);
                }
            }

            else if (z < 0)
            {
                if(csouth)
                {
                    transpid = this.SouthNeighbor.GetBlockIndex(x, y, 15);
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

            return transblock.Transparent;
        }

#endregion
    }
}
