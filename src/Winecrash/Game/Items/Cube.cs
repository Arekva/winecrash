using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WEngine;

namespace Winecrash
{
    public class Cube : Block
    {
        //-- set in item config --//
        public CubeTexturePaths Textures { get; set; }

        public override Quaternion InventoryRotation { get; set; } = new Quaternion(-15, 47, -15.4);
        
        
        public override Vector3D HandPosition { get; set; } = Vector3D.Forward * 1.25D + Vector3D.Left * 1.25 + Vector3D.Down * 1D;
        public override Quaternion HandRotation { get; set; } = new Quaternion(0, -55, 0);
        public override Vector3D HandScale { get; set; } = Vector3D.One * 0.75;

        private static Vector3F[] _CubeVertices = new Vector3F[36]
        {
            // up face
            new Vector3F(-0.5F, 0.5F, -0.5F),
            new Vector3F(-0.5F, 0.5F, 0.5F),
            new Vector3F(0.5F, 0.5F, -0.5F),
            new Vector3F(0.5F, 0.5F, -0.5F),
            new Vector3F(-0.5F, 0.5F, 0.5F),
            new Vector3F(0.5F, 0.5F, 0.5F),

            // down face
            new Vector3F(-0.5F, -0.5F, 0.5F),
            new Vector3F(-0.5F, -0.5F, -0.5F),
            new Vector3F(0.5F, -0.5F, 0.5F),
            new Vector3F(0.5F, -0.5F, 0.5F),
            new Vector3F(-0.5F, -0.5F, -0.5F),
            new Vector3F(0.5F, -0.5F, -0.5F),

            // west face
            new Vector3F(-0.5F, -0.5F, -0.5F),
            new Vector3F(-0.5F, -0.5F, 0.5F),
            new Vector3F(-0.5F, 0.5F, -0.5F),
            new Vector3F(-0.5F, 0.5F, -0.5F),
            new Vector3F(-0.5F, -0.5F, 0.5F),
            new Vector3F(-0.5F, 0.5F, 0.5F),

            // east face
            new Vector3F(0.5F, 0.5F, 0.5F),
            new Vector3F(0.5F, -0.5F, 0.5F),
            new Vector3F(0.5F, 0.5F, -0.5F),
            new Vector3F(0.5F, 0.5F, -0.5F),
            new Vector3F(0.5F, -0.5F, 0.5F),
            new Vector3F(0.5F, -0.5F, -0.5F),

            // ~the~ north face
            new Vector3F(-0.5F, -0.5F, 0.5F),
            new Vector3F(0.5F, -0.5F, 0.5F),
            new Vector3F(-0.5F, 0.5F, 0.5F),
            new Vector3F(-0.5F, 0.5F, 0.5F),
            new Vector3F(0.5F, -0.5F, 0.5F),
            new Vector3F(0.5F, 0.5F, 0.5F),

            // south face
            new Vector3F(0.5F, 0.5F, -0.5F),
            new Vector3F(0.5F, -0.5F, -0.5F),
            new Vector3F(-0.5F, 0.5F, -0.5F),
            new Vector3F(-0.5F, 0.5F, -0.5F),
            new Vector3F(0.5F, -0.5F, -0.5F),
            new Vector3F(-0.5F, -0.5F, -0.5F)
        };
        
        private static Vector3F up = Vector3F.Up;
        private static Vector3F down = Vector3F.Down;
        private static Vector3F left = Vector3F.Left;
        private static Vector3F right = Vector3F.Right;
        private static Vector3F forward = Vector3F.Forward;
        private static Vector3F south = Vector3F.Backward;

        private static Vector3F[] _CubeNormals = new Vector3F[36]
        {
            up, up, up, up, up, up,
            down, down, down, down, down, down,
            left, left, left, left, left, left,
            right, right, right, right, right, right,
            forward, forward, forward, forward, forward, forward,
            south, south, south, south, south, south
        };

        private static uint[] _CubeIndices = new uint[36]
        {
            0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,
            19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35
        };

        static Cube()
        {
            /*for (int i = 0; i < _CubeVertices.Length; i++)
            {
                _CubeVertices[i] = new Quaternion(-15,47,-15.4) * _CubeVertices[i];
            }*/
        }
        
        [JsonIgnore] public Texture UpTexture { get; private set; }
        [JsonIgnore] public Texture DownTexture { get; private set; }
        [JsonIgnore] public Texture EastTexture { get; private set; }
        [JsonIgnore] public Texture WestTexture { get; private set; }
        [JsonIgnore] public Texture NorthTexture { get; private set; }
        [JsonIgnore] public Texture SouthTexture { get; private set; }

        public override void OnDeserialize()
        {
            if (Engine.DoGUI)
            {
                UpTexture = String.IsNullOrEmpty(Textures.Up) ? Texture.Blank : Texture.GetOrCreate(Textures.Up);
                DownTexture = String.IsNullOrEmpty(Textures.Down) ? Texture.Blank : Texture.GetOrCreate(Textures.Down);

                EastTexture = String.IsNullOrEmpty(Textures.East) ? Texture.Blank : Texture.GetOrCreate(Textures.East);
                WestTexture = String.IsNullOrEmpty(Textures.West) ? Texture.Blank : Texture.GetOrCreate(Textures.West);

                NorthTexture = String.IsNullOrEmpty(Textures.North)
                    ? Texture.Blank
                    : Texture.GetOrCreate(Textures.North);
                SouthTexture = String.IsNullOrEmpty(Textures.South)
                    ? Texture.Blank
                    : Texture.GetOrCreate(Textures.South);
            }

            base.OnDeserialize();
        }

        internal override Mesh BuildModel()
        {
            if (this.Texture) return base.BuildModel();
            else
            {
                float nbItems = ItemCache.TotalItems;
                float iindex = ItemCache.GetIndex(this.Identifier);

                float yPercent = iindex;

                Vector2F shift = new Vector2F(0, yPercent);
                Vector2F scale = new Vector2D(1F / 6F, 1F / nbItems);

                Vector2F[] uvs = new Vector2F[36]
                {
                    // up
                    new Vector2F(scale.X * 2, scale.Y * yPercent),
                    new Vector2F(scale.X * 2, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 3, scale.Y * yPercent),
                    new Vector2F(scale.X * 3, scale.Y * yPercent),
                    new Vector2F(scale.X * 2, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 3, scale.Y * (yPercent+1)),
                    
                    // down
                    new Vector2F(scale.X * 4, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 4, scale.Y * yPercent),
                    new Vector2F(scale.X * 3, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 3, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 4, scale.Y * yPercent),
                    new Vector2F(scale.X * 3, scale.Y * yPercent),
                    
                    // west
                    new Vector2F(scale.X * 2, scale.Y * yPercent),
                    new Vector2F(scale.X * 1, scale.Y * yPercent),
                    new Vector2F(scale.X * 2, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 2, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 1, scale.Y * yPercent),
                    new Vector2F(scale.X * 1, scale.Y * (yPercent+1)),
                    
                    // east
                    new Vector2F(scale.X * 1, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 1, scale.Y * yPercent),
                    new Vector2F(scale.X * 0, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 0, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 1, scale.Y * yPercent),
                    new Vector2F(scale.X * 0, scale.Y * yPercent),
                    
                    // north
                    new Vector2F(scale.X * 5, scale.Y * yPercent),
                    new Vector2F(scale.X * 4, scale.Y * yPercent),
                    new Vector2F(scale.X * 5, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 5, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 4, scale.Y * yPercent),
                    new Vector2F(scale.X * 4, scale.Y * (yPercent+1)),
                    
                    // south
                    new Vector2F(scale.X * 6, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 6, scale.Y * yPercent),
                    new Vector2F(scale.X * 5, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 5, scale.Y * (yPercent+1)),
                    new Vector2F(scale.X * 6, scale.Y * yPercent),
                    new Vector2F(scale.X * 5, scale.Y * yPercent),
                };

                Mesh m = new Mesh(this.Identifier + " 3D Render [Cube]")
                {
                    Vertices = _CubeVertices,
                    UVs = uvs,
                    Normals = _CubeNormals,
                    Triangles = _CubeIndices,
                    Tangents = new Vector4F[36]
                };
                m.Apply(true);

                return m;
            }
        }
    }
}