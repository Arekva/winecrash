using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WEngine;
using Winecrash.Entities;
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
    
    public sealed partial class Chunk : Module, ICollider
    {
        public override string ToString()
        {
            return $"Chunk[{Coordinates.X};{Coordinates.Y}/ {Dimension.Identifier}]";
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
        
        public object EntityLocker = new object();
        public List<Entity> Entities { get; set; } = new List<Entity>();

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

        public volatile MeshRenderer SolidRenderer;
        public volatile MeshRenderer TransparentRenderer;

        public bool BuildEndFrame { get; set; } = false;

        public bool ConstructedOnce = false;
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

            //this.RunAsync = true;

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
                // FAST DEBUG - FLAT WORLD -
                if(NorthNeighbor != null) NorthNeighbor.BuildEndFrame = true;
                if(SouthNeighbor != null) SouthNeighbor.BuildEndFrame = true;
                if(WestNeighbor != null) WestNeighbor.BuildEndFrame = true;
                if(EastNeighbor != null) EastNeighbor.BuildEndFrame = true;
            }
        }

        private void CreateRenderer()
        {
            if (ConstructedOnce) return;
            
            if (!SolidRenderer)
            {
                SolidRenderer = this.WObject.AddModule<MeshRenderer>();
                SolidRenderer.Material = ItemCache.AtlasMaterial;
            }

            if (!TransparentRenderer)
            {
                TransparentRenderer = this.WObject.AddModule<MeshRenderer>();
                TransparentRenderer.Material = ItemCache.AtlasMaterial;
                TransparentRenderer.DrawOrder = 1;
            }
        }

        protected override void LateUpdate()
        {
            if (Engine.DoGUI)
            {
                if (BuildEndFrame)
                {
                    BuildEndFrame = false;

                    Construct();
                }
            }
            
            // if local, delete unnecessary chunks
            /*if (Player.LocalPlayer != null)
            {
                Vector2I pcords = Player.LocalPlayer.Entity.ChunkCoordinates;

                double dist = Vector2I.Distance(pcords, this.Coordinates);

                if (dist > Winecrash.RenderDistance + 2)
                {
                    World.UnloadChunk(this);
                    return;
                }
            }*/

            base.LateUpdate();
        }

        protected override void OnDelete()
        {
            OnChunkLoad -= OnChunkLoadedDelegate;
            OnChunkUnload -= OnChunkUnloadDelegate;
            
            OnChunkUnload?.Invoke(new ChunkEventArgs(this));

            lock (EntityLocker)
            {
                foreach (Entity ent in this.Entities)
                {
                    ent.Delete();
                }
                
                this.Entities.Clear();
                this.Entities = null;
            }
            
            this.NorthNeighbor = null;
            this.SouthNeighbor = null;
            this.WestNeighbor = null;
            this.EastNeighbor = null;
            this.Blocks = null;


            this.SolidRenderer?.Delete();
            this.SolidRenderer = null;
            this.TransparentRenderer?.Delete();
            this.TransparentRenderer = null;

            
            base.OnDelete();
        }
#endregion

        public void Edit(int x, int y, int z, Block b = null)
        {
            PrivateEdit(x,y,z,b);
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
    }
}
