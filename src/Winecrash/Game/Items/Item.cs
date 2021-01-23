using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WEngine;
using Winecrash.Entities;

namespace Winecrash
{
    public class Item : IEquatable<Item>
    {
        //-- set in database config --//
        [JsonIgnore] public string Identifier { get; internal set; } = "winecrash:air";
        
        //-- set in item config --//
        public string DisplayName { get; set; } = "#winecrash:error.noname";
        
        //-- set in item config --//
        public Color256 DisplayColor { get; set; } = Color256.White;

        //-- set in item config --//
        public string TexturePath { get; set; } = null;

        //-- set in item config --//
        public byte Stack { get; set; } = 64;

        //-- set in item config --//
        public virtual Vector3D HandPosition { get; set; } = Vector3D.Forward * 0.9 + Vector3D.Left + Vector3D.Down * 0.6;
        
        //-- set in item config --//
        public virtual Quaternion HandRotation { get; set; } = new Quaternion(0,90,0) *  new Quaternion(0,180,0);
        
        //-- set in item config --//
        public virtual Vector3D HandScale { get; set; } = Vector3D.One;
        
        //-- set in item config --//
        public virtual Vector3D InventoryScale { get; set; } = Vector3D.One * 1.1;
        
        //-- set in item config --//
        public virtual Quaternion InventoryRotation { get; set; } = Quaternion.Identity;
        

        [JsonIgnore] public Mesh Model { get; set; } = null;
        [JsonIgnore] public Texture Texture { get; set; } = null;


        public virtual void OnDeserialize()
        {
            if (Engine.DoGUI)
            {
                if (!string.IsNullOrEmpty(TexturePath))
                    Texture = WEngine.Texture.GetOrCreate(TexturePath);

                this.Model = this.BuildModel();
            }
        }

        internal virtual Mesh BuildModel()
        {
            if (this.Identifier == "winecrash:air")
            {
                return PlayerEntity.PlayerRightArmMesh;
            }
            
            Texture texture = this.Texture ?? Texture.Blank;

            List<Vector3F> vertices = new List<Vector3F>();
            List<Vector2F> uvs = new List<Vector2F>();
            List<Vector3F> normals = new List<Vector3F>();
            List<uint> triangles = new List<uint>();

            Stack<BlockFaces> faces = new Stack<BlockFaces>(6);
            uint triangle = 0;
            Block block = null;
            ushort index = 0;

            Vector3F deltas = new Vector3F(1.0F / texture.Size.X, 1.0F / texture.Size.Y);
            deltas.Z = Math.Min(deltas.X, deltas.Y);


            float nbItems = ItemCache.TotalItems;
            float iindex = ItemCache.GetIndex(this.Identifier);

            float yPercent = iindex;


            Vector2F shift = new Vector2F(0,yPercent);
            Vector2F scale = new Vector2D(1F/6F, 1F / nbItems);

            Vector3F up = Vector3F.Up;
            Vector3F down = Vector3F.Down;
            Vector3F left = Vector3F.Left;
            Vector3F right = Vector3F.Right;
            Vector3F forward = Vector3F.Forward;
            Vector3F south = Vector3F.Backward;

            void CreateUVs(float minXPos, float minYPos, BlockFaces face)
            {
                float maxXPos = minXPos + deltas.X;
                float maxYPos = minYPos + deltas.Y;
                
                switch (face)
                {
                    case BlockFaces.Up:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F(minXPos, minYPos + yPercent) * scale, //0
                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //incr
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //2

                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //3
                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //4
                            new Vector2F(maxXPos, maxYPos + yPercent) * scale, //5
                        });
                    }
                        break;

                    case BlockFaces.Down:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F(maxXPos, maxYPos + yPercent) * scale, //top left
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //bottom left
                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //top right

                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //top right
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //bottom left
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //top left
                        });
                    }
                        break;

                    case BlockFaces.North:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //5
                            new Vector2F(minXPos, minYPos + yPercent) * scale, //4
                            new Vector2F(maxXPos, maxYPos + yPercent) * scale, //3

                            new Vector2F(maxXPos, maxYPos + yPercent) * scale, //2
                            new Vector2F(minXPos, minYPos + yPercent) * scale, //incr
                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //0
                        });
                    }
                        break;

                    case BlockFaces.South:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F(maxXPos, maxYPos + yPercent) * scale, //0 topleft
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //2 bottom left
                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //incr top right

                            new Vector2F(minXPos, maxYPos + yPercent) * scale, // 4 top right
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //3 bottom left
                            new Vector2F(minXPos, minYPos + yPercent) * scale, //5 bottom right
                        });
                    }
                        break;


                    case BlockFaces.West:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //5
                            new Vector2F(minXPos, minYPos + yPercent) * scale, //4
                            new Vector2F(maxXPos, maxYPos + yPercent) * scale, //3

                            new Vector2F(maxXPos, maxYPos + yPercent) * scale, //2
                            new Vector2F(minXPos, minYPos + yPercent) * scale, //incr
                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //0
                        });
                    }
                        break;

                    case BlockFaces.East:
                    {
                        uvs.AddRange(new[]
                        {
                            new Vector2F(maxXPos, maxYPos + yPercent) * scale, //5
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //3
                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //4

                            new Vector2F(minXPos, maxYPos + yPercent) * scale, //incr
                            new Vector2F(maxXPos, minYPos + yPercent) * scale, //2
                            new Vector2F(minXPos, minYPos + yPercent) * scale, //0
                        });
                    }
                        break;
                }
            }
            void CreateVertices(float minXPos, float minYPos, BlockFaces face)
            {
                float maxXPos = minXPos + deltas.X;
                float maxYPos = minYPos + deltas.Y;
                float halfZdelta = deltas.Z / 2.0F;

                const float scaleFactor = 1.5F;

                switch (face)
                {
                    case BlockFaces.Up:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, maxYPos - 0.5F, halfZdelta) * scaleFactor
                        });
                    }
                        break;

                    case BlockFaces.Down:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(minXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, minYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, minYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, -halfZdelta) * scaleFactor
                        });
                    }
                        break;

                    case BlockFaces.West:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(minXPos - 0.5F, minYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, halfZdelta) * scaleFactor
                        });
                    }
                        break;

                    case BlockFaces.East:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(maxXPos - 0.5F, maxYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, -halfZdelta) * scaleFactor
                        });
                    }
                        break;

                    case BlockFaces.North:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(minXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, maxYPos - 0.5F, halfZdelta) * scaleFactor
                        });
                    }
                        break;

                    case BlockFaces.South:
                    {
                        vertices.AddRange(new[]
                        {
                            new Vector3F(maxXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, maxYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(maxXPos - 0.5F, minYPos - 0.5F, -halfZdelta) * scaleFactor,
                            new Vector3F(minXPos - 0.5F, minYPos - 0.5F, -halfZdelta) * scaleFactor
                        });
                    }
                        break;
                }
            }
            void CreateNormals(BlockFaces face)
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

            for (int y = 0; y < texture.Size.Y; y++)
            {
                for (int x = 0; x < texture.Size.X; x++)
                {
                    if (!IsTransparent(texture, x, y))
                    {
                        float xminpos = deltas.X * x;
                        float yminpos = deltas.Y * y;

                        float xmaxpos = xminpos + deltas.X;
                        float xmaspos = yminpos + deltas.Y;

                        if (IsTransparent(texture, x, y + 1)) faces.Push(BlockFaces.Up);
                        if (IsTransparent(texture, x, y - 1)) faces.Push(BlockFaces.Down);
                        if (IsTransparent(texture, x - 1, y)) faces.Push(BlockFaces.West);
                        if (IsTransparent(texture, x + 1, y)) faces.Push(BlockFaces.East);

                        faces.Push(BlockFaces.North);
                        faces.Push(BlockFaces.South);

                        foreach (BlockFaces face in faces)
                        {
                            CreateVertices(xminpos, yminpos, face);
                            CreateUVs(xminpos, yminpos, face);
                            CreateNormals(face);

                            triangles.AddRange(new uint[6]
                                {triangle, triangle + 1, triangle + 2, triangle + 3, triangle + 4, triangle + 5});
                            triangle += 6;
                        }

                        faces.Clear();
                    }
                }
            }

            Mesh mesh = null;

            if (vertices.Count != 0)
            {
                mesh = new Mesh(this.Identifier + " 3D Render [Standard]")
                {
                    Vertices = vertices.ToArray(),
                    Triangles = triangles.ToArray(),
                    UVs = uvs.ToArray(),
                    Normals = normals.ToArray(),
                    Tangents = new Vector4F[vertices.Count]
                };

                mesh.Apply(true);
            }

            vertices = null;
            triangles = null;
            uvs = null;
            normals = null;

            return mesh;
        }

        private static bool IsTransparent(Texture texture, int x, int y, int threshold = 255)
        {
            if (x < 0 || x > texture.Size.X - 1) return true;
            if (y < 0 || y > texture.Size.Y - 1) return true;
            return texture[x, y].A < threshold;
        }

        public bool Equals(Item other)
        {
            return other != null && other.Identifier == this.Identifier;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Item);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        public void Delete()
        {
            this.Model?.Delete();
        }

        public override string ToString()
        {
            return $"{this.Identifier} ({this.GetType().Name})";
        }
    }
}