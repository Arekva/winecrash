using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Diagnostics;

using OpenTK.Graphics.OpenGL4;

namespace Winecrash.Engine
{
    public sealed class Mesh : BaseObject
    {
        public Mesh() : base() {}

        public Mesh(string name) : base(name) { }

        public Mesh(Mesh original) : base()
        {
            this.Vertices = original.Vertices;
            this.Triangles = original.Triangles;
            this.UVs = original.UVs;
            this.Tangents = original.Tangents;
            this.Normals = original.Normals;

            this.Apply(true);
        }

        public Vector3F[] Vertices { get; set; }
        public UInt32[] Triangles { get; set; }
        public Vector2F[] UVs { get; set; }
        public Vector4F[] Tangents { get; set; }
        public Vector3F[] Normals { get; set; }

        internal float[] Vertex { get; private set; } = null;
        internal uint[] Indices { get; private set; } = null;

        internal bool AskedForApply { get; set; } = false;

        public delegate void MeshDeleteDelegate();

        public event MeshDeleteDelegate OnDelete;

        public void Apply(bool deleteWorkArrays)
        {
            Vertex = new float[this.Vertices.Length * 8];

            for (int vert = 0; vert < this.Vertices.Length; vert++)
            {
                Vector3F vertice = this.Vertices[vert];
                Vertex[vert * 8 + 0] = vertice.X;
                Vertex[vert * 8 + 1] = vertice.Y;
                Vertex[vert * 8 + 2] = vertice.Z;

                Vector2F uvs = this.UVs[vert];
                Vertex[vert * 8 + 3] = uvs.X;
                Vertex[vert * 8 + 4] = uvs.Y;

                Vector3F normal = this.Normals[vert];

                Vertex[vert * 8 + 5] = normal.X;
                Vertex[vert * 8 + 6] = normal.Y;
                Vertex[vert * 8 + 7] = normal.Z;
            }

            //Indices = Triangles;

            Indices = this.Triangles/*.Reverse().ToArray()*/;

            if(deleteWorkArrays)
            {
                Vertices = null;
                Triangles = null;
                UVs = null;
                Tangents = null;
                Normals = null;
            }
            

            AskedForApply = true;
        }

        public static Mesh LoadFile(string path, MeshFormats format)
        {
            return format switch
            {
                MeshFormats.Wavefront => LoadWavefront(path),
                MeshFormats.Blender => LoadBlender(path),
                _ => null,
            };
        }

        private static Mesh LoadWavefront(string path)
        {
            return ModelLoaders.WavefrontLoader.ImportFile(path);
        }
        private static Mesh LoadBlender(string path)
        {
            throw new NotImplementedException("*.Blend Blender format is not supported yet.");
        }
        internal override void ForcedDelete()
        {
            this.Delete();
            base.ForcedDelete();
        }
        public override void Delete()
        {
            this.Vertices = null;
            this.Triangles = null;
            this.UVs = null;
            this.Tangents = null;
            this.Normals = null;

            this.Vertex = null;
            this.Indices = null;

            OnDelete?.Invoke();
            OnDelete = null;

            base.Delete();
        }

        public override bool Equals(object obj)
        {
            return obj is Mesh mesh &&
                   base.Equals(obj) &&
                   EqualityComparer<Vector3F[]>.Default.Equals(Vertices, mesh.Vertices) &&
                   EqualityComparer<uint[]>.Default.Equals(Triangles, mesh.Triangles) &&
                   EqualityComparer<Vector2F[]>.Default.Equals(UVs, mesh.UVs) &&
                   EqualityComparer<Vector4F[]>.Default.Equals(Tangents, mesh.Tangents) &&
                   EqualityComparer<Vector3F[]>.Default.Equals(Normals, mesh.Normals);
        }
    }
}
