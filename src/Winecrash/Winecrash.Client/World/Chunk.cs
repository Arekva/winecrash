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
        public const int BlockResolution = 16;
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
            // this.Blocks = Generator.GetChunk(this.Ticket.Position.X, this.Ticket.Position.Y, out _);
            MeshRenderer mr = this.WObject.AddModule<MeshRenderer>();
            mr.Mesh = new Mesh(World.DebugChunkMesh);
            mr.Material = new Material(Shader.Find("Standard"));
            mr.Material.SetData<int>("debug", 1);
        }

        protected override void Start()
        {
            this.WObject.LocalPosition = new Vector3F(this.Ticket.Position.X * Chunk.Width, 0, this.Ticket.Position.Y * Chunk.Depth);
            this.Blocks = Generator.GetChunk(this.Ticket.Position.X, this.Ticket.Position.Y, out _);

            BuildChunk();
        }
        public int Get1DIndex(int x, int y, int z)
        {
            return x + Width * y + Width * Height * z;
        }


        public void BuildChunk()
        {
            List<Vector3F> vertices = new List<Vector3F>();
            List<Vector2F> uv = new List<Vector2F>();
            List<Vector3F> normals = new List<Vector3F>();
            List<uint> triangles = new List<uint>();
            //List<Vector4F> tangents = null;


            //No texture editing yet

            //Vector2Int TextureSize = new Vector2Int(Item.ItemAtlas.width, Item.ItemAtlas.height);
            //Vector2 TextureSizeF = new Vector2(TextureSize.x, TextureSize.y);


            //No tangents yet

            //Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
            //Vector4[] tan = new Vector4[6] { tangent, tangent, tangent, tangent, tangent, tangent };

            Stack<BlockFaces> faces = new Stack<BlockFaces>();

            uint triangle = 0;

            const int incr = 1;

            for (int z = 0; z < Depth; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        // Get the ID of the block
                        ItemDef def = ItemCache.Get(this.Blocks[x, y, z]);

                        if (def.Identifier == "winecrash:air") continue;

                        //todo: later use CubeDef
                        //if (def is CubeDef cd) {...}

                        if (Transparent(x, y + incr, z)) faces.Push(BlockFaces.Up);
                        if (Transparent(x, y - incr, z)) faces.Push(BlockFaces.Down);
                        if (Transparent(x - incr, y, z)) faces.Push(BlockFaces.West);
                        if (Transparent(x + incr, y, z)) faces.Push(BlockFaces.East);
                        if (Transparent(x, y, z + incr)) faces.Push(BlockFaces.North);
                        if (Transparent(x, y, z - incr)) faces.Push(BlockFaces.South);

                        foreach (BlockFaces face in faces)
                        {
                            CreateFace(0, in face, in x, in y, in z, vertices, uv, normals);
                            //tangents.AddRange(tan);
                            triangles.AddRange(new uint[6] { triangle, triangle + 1, triangle + 2, triangle + 3, triangle + 4, triangle + 5 });
                            triangle += 6;
                        }

                        faces.Clear();
                    }
                }
            }

            if (vertices.Count != 0)
            {
                Mesh m = new Mesh("Chunk mesh")
                {
                    Vertices = vertices.ToArray(),
                    Triangles = triangles.ToArray(),
                    UVs = uv.ToArray(),
                    Normals = normals.ToArray(),
                    Tangents = new Vector4F[vertices.Count]
                };

                m.Apply();


                MeshRenderer mr = this.WObject.GetModule<MeshRenderer>();
                mr.Mesh.Delete();
                mr.Mesh = m;

                //Debug.Log("Created chunk mesh for " + this.WObject);
            }
        }
        private static void CreateFace(
            in uint id, in BlockFaces face,
            in int x, in int y, in int z,
            List<Vector3F> vertices, List<Vector2F> uv, List<Vector3F> normals
            )
        {
            vertices.AddRange(CreateVertices(in x, in y, in z, in face));
            uv.AddRange(CreateUVs(in id, in face));
            normals.AddRange(CreateNormals(in face));
        }


        private static Vector3F[] CreateNormals(in BlockFaces direction)
        {
            Vector3F[] norms;

            Vector3F up = Vector3F.Up;
            Vector3F down = Vector3F.Down;
            Vector3F left = Vector3F.Left;
            Vector3F right = Vector3F.Right;
            Vector3F forward = Vector3F.Forward;
            Vector3F south = Vector3F.Backward;

            switch (direction)
            {
                case BlockFaces.Up:
                    {
                        norms = new Vector3F[6]
                        {
                            up, up, up, up, up, up
                        };
                    }
                    break;

                case BlockFaces.Down:
                    {
                        norms = new Vector3F[6]
                        {
                            down, down, down, down, down, down
                        };
                    }
                    break;

                case BlockFaces.West:
                    {
                        norms = new Vector3F[6]
                        {
                            left, left, left, left, left, left
                        };
                    }
                    break;

                case BlockFaces.East:
                    {
                        norms = new Vector3F[6]
                        {
                            right, right, right, right, right, right
                        };
                    }
                    break;

                case BlockFaces.North:
                    {
                        norms = new Vector3F[6]
                        {
                            forward, forward, forward, forward, forward, forward
                        };
                    }
                    break;

                case BlockFaces.South:
                    {
                        norms = new Vector3F[6]
                        {
                            south, south, south, south, south, south
                        };
                    }
                    break;

                default: //up
                    {
                        norms = new Vector3F[6]
                        {
                            up, up, up, up, up, up
                        };
                    }
                    break;
            }

            return norms;
        }
        /// <summary>
        /// Get the vertices of a specific face by its direction.
        /// </summary>
        /// <param name="x">The X position of the block.</param>
        /// <param name="y">The Y position of the block.</param>
        /// <param name="z">The Z position of the block.</param>
        /// <param name="direction">The direction the face is facing to.</param>
        /// <returns></returns>
        private static Vector3F[] CreateVertices(in int x, in int y, in int z, in BlockFaces direction)
        {
            Vector3F[] verts = null;

            const int incr = 1;

            switch (direction)
            {
                case BlockFaces.Up:
                    {
                        verts = new Vector3F[6]
                        {
                        new Vector3F(x, y + incr, z),
                        new Vector3F(x, y + incr, z + incr),
                        new Vector3F(x + incr, y + incr, z),
                        new Vector3F(x + incr, y + incr, z),
                        new Vector3F(x, y + incr, z + incr),
                        new Vector3F(x + incr, y + incr, z + incr)
                        };
                    }
                    break;

                case BlockFaces.Down:
                    {
                        verts = new Vector3F[6]
                        {
                        new Vector3F(x, y, z + incr),
                        new Vector3F(x, y, z),
                        new Vector3F(x + incr, y, z + incr),
                        new Vector3F(x + incr, y, z + incr),
                        new Vector3F(x, y, z),
                        new Vector3F(x + incr, y, z)
                        };
                    }
                    break;

                case BlockFaces.West:
                    {
                        verts = new Vector3F[6]
                        {
                        new Vector3F(x, y, z),
                        new Vector3F(x, y, z + incr),
                        new Vector3F(x, y + incr, z),
                        new Vector3F(x, y + incr, z),
                        new Vector3F(x, y, z + incr),
                        new Vector3F(x, y + incr, z + incr)
                        };
                    }
                    break;

                case BlockFaces.East:
                    {
                        verts = new Vector3F[6]
                        {
                        new Vector3F(x + incr, y + incr, z + incr),
                        new Vector3F(x + incr, y, z + incr),
                        new Vector3F(x + incr, y + incr, z),
                        new Vector3F(x + incr, y + incr, z),
                        new Vector3F(x + incr, y, z + incr),
                        new Vector3F(x + incr, y, z)
                        };
                    }
                    break;

                case BlockFaces.North:
                    {
                        verts = new Vector3F[6]
                        {
                        new Vector3F(x, y, z + incr),
                        new Vector3F(x + incr, y, z + incr),
                        new Vector3F(x, y + incr, z + incr),
                        new Vector3F(x, y + incr, z + incr),
                        new Vector3F(x + incr, y, z + incr),
                        new Vector3F(x + incr, y + incr, z + incr)
                        };
                    }
                    break;

                case BlockFaces.South:
                    {
                        verts = new Vector3F[6]
                        {
                        new Vector3F(x + incr, y + incr, z),
                        new Vector3F(x + incr, y, z),
                        new Vector3F(x, y + incr, z),
                        new Vector3F(x, y + incr, z),
                        new Vector3F(x + incr, y, z),
                        new Vector3F(x, y, z)
                        };
                    }
                    break;
            }

            return verts;
        }

        /// <summary>
        /// Get the UVs of the block.
        /// </summary>
        /// <param name="index">The index of the block.</param>
        /// <param name="direction">The direction of the block.</param>
        /// <returns></returns>
        private static Vector2F[] CreateUVs(in uint id, in BlockFaces direction)
        {
            return new Vector2F[6];
            Vector2F TextureSize;
            Vector2F[] uvs = null;
            Vector2F position = new Vector2F((int)direction * BlockResolution, id * BlockResolution);

            int xPos = (int)direction * BlockResolution;
            int yPos = (int)id * BlockResolution;
            float xTex = TextureSize.X;
            float yTex = TextureSize.Y;
            switch (direction)
            {
                case BlockFaces.Up:
                    {
                        uvs = new Vector2F[6]
                        {
                        (position / TextureSize),
                        new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                        new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex)
                        };
                    }
                    break;

                case BlockFaces.Down:
                    {
                        uvs = new Vector2F[6]
                        {
                        new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        (position / TextureSize),
                        new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex),
                        new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex),
                        (position / TextureSize),
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                        };
                    }
                    break;

                case BlockFaces.West:
                    {
                        uvs = new Vector2F[6]
                        {
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                            (position / TextureSize),
                            new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex),
                            new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex),
                            (position / TextureSize),
                            new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        };
                    }
                    break;

                case BlockFaces.East:
                    {
                        uvs = new Vector2F[6]
                        {
                        new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex),
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                        new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                        (position / TextureSize)
                        };
                    }
                    break;

                case BlockFaces.North:
                    {
                        uvs = new Vector2F[6]
                        {
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                        (position / TextureSize),
                        new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex),
                        new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex),
                        (position / TextureSize),
                        new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        };
                    }
                    break;

                case BlockFaces.South:
                    {
                        uvs = new Vector2F[6]
                        {
                        new Vector2F((xPos + BlockResolution) / xTex, (yPos + BlockResolution) / yTex),
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                        new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        new Vector2F((xPos / xTex), ((yPos + BlockResolution) / yTex)),
                        new Vector2F((xPos + BlockResolution) / xTex, yPos / yTex),
                        (position / TextureSize),
                        };
                    }
                    break;
            }

            return uvs;
        }

        /// <summary>
        /// Get the transparency setting of a block within the chunnk or neighbors.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private bool Transparent(int x, int y, int z)
        {
            //If the block is within the chunk
            if (IndexesInRange(x, y, z))
            {
                return ItemCache.Get(Blocks[x, y, z]).Identifier == "winecrash:air";
            }

            else return true;

            /*else
            {
                if (x < 0 && _Neighboors[2] != null)
                {
                    return Item.Get<Cube>(_Neighboors[2][Size - 1, y, z].ID).Settings.Transparent;
                }

                if (x > Size - 1 && _Neighboors[3] != null)
                {
                    return Item.Get<Cube>(_Neighboors[3][0, y, z].ID).Settings.Transparent;
                }

                if (y < 0 && _Neighboors[1] != null)
                {
                    return Item.Get<Cube>(_Neighboors[1][x, Size - 1, z].ID).Settings.Transparent;
                }

                if (y > Size - 1 && _Neighboors[0] != null)
                {
                    return Item.Get<Cube>(_Neighboors[0][x, 0, z].ID).Settings.Transparent;
                }

                if (z < 0 && _Neighboors[5] != null)
                {
                    return Item.Get<Cube>(_Neighboors[5][x, y, Size - 1].ID).Settings.Transparent;
                }
                if (z > Size - 1 && _Neighboors[4] != null)
                {
                    return Item.Get<Cube>(_Neighboors[4][x, y, 0].ID).Settings.Transparent;
                }

                return false;
            }*/
        }

        private static bool IndexesInRange(int x, int y, int z)
        {
            return (x > -1 && x < Width && y > -1 && y < Height && z > -1 && z < Depth);
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
