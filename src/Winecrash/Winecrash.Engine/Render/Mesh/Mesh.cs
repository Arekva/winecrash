using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Diagnostics;

using OpenTK.Graphics.OpenGL4;
using System.Windows.Forms;

namespace Winecrash.Engine
{
    public sealed class Mesh : BaseObject
    {
        public Mesh() : base() 
        {
            Cache.Add(this);
        }

        public Mesh(string name) : base(name) 
        {
            Cache.Add(this);
        }

        public Mesh(Mesh original) : base()
        {
            this.Vertices = original.Vertices;
            this.Triangles = original.Triangles;
            this.UVs = original.UVs;
            this.Tangents = original.Tangents;
            this.Normals = original.Normals;

            Cache.Add(this);


            this.Apply(true);
        }

        internal int VertexBufferObject = -1;
        internal int ElementBufferObject = -1;
        internal int VertexArrayObject = -1;

        private readonly static object applyLocker = new object();

        public Vector3F[] Vertices { get; set; }
        public UInt32[] Triangles { get; set; }
        public Vector2F[] UVs { get; set; }
        public Vector4F[] Tangents { get; set; }
        public Vector3F[] Normals { get; set; }

        internal float[] Vertex { get; set; } = null;
        internal uint Indices { get; private set; } = 0;

        public delegate void MeshDeleteDelegate();

        public event MeshDeleteDelegate OnDelete;

        internal static List<Mesh> Cache = new List<Mesh>();

        internal void ApplySafe(bool deleteWorkArrays)
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

            Indices = (uint)this.Triangles.Length;

            if (deleteWorkArrays)
            {
                Vertices = null;
                UVs = null;
                Tangents = null;
                Normals = null;
            }

            if (VertexArrayObject == -1)
            {
                VertexBufferObject = GL.GenBuffer();
                ElementBufferObject = GL.GenBuffer();
                VertexArrayObject = GL.GenVertexArray();
            }


            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.Length * sizeof(float), Vertex, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (int)Indices * sizeof(uint), this.Triangles, BufferUsageHint.StaticDraw);

            if (deleteWorkArrays)
            {
                Triangles = null;
            }

            this.Vertex = null;
        }

        bool alreadyBuilding = false;
        bool abort = false;
        public void Apply(bool deleteWorkArrays = false)
        {
            if (alreadyBuilding) abort = true;
            alreadyBuilding = true;
            if (this.Vertices == null)
            {
                Debug.LogWarning($"Cannot apply Mesh \"{this.Name}\": no vertices.");
                return;
            }

            if (abort)
            {
                abort = false;
                return;
            }

            float[] vertex = new float[this.Vertices.Length * 8];

            for (int vert = 0; vert < this.Vertices.Length; vert++)
            {
                if(abort)
                {
                    abort = false;
                    return;
                }
                Vector3F vertice = this.Vertices[vert];
                vertex[vert * 8 + 0] = vertice.X;
                vertex[vert * 8 + 1] = vertice.Y;
                vertex[vert * 8 + 2] = vertice.Z;

                Vector2F uvs = this.UVs[vert];
                vertex[vert * 8 + 3] = uvs.X;
                vertex[vert * 8 + 4] = uvs.Y;

                Vector3F normal = this.Normals[vert];

                vertex[vert * 8 + 5] = normal.X;
                vertex[vert * 8 + 6] = normal.Y;
                vertex[vert * 8 + 7] = normal.Z;
            }

            if (abort)
            {
                abort = false;
                return;
            }
            Indices = (uint)this.Triangles.Length;
            if (abort)
            {
                abort = false;
                return;
            }
            if (deleteWorkArrays)
            {
                Vertices = null;
                UVs = null;
                Tangents = null;
                Normals = null;
            }
            if (abort)
            {
                abort = false;
                return;
            }

            lock (applyLocker)
            {
                Vertex = vertex;
            }

            Graphics.Window.InvokeRender(() =>
            {
                if (abort)
                {
                    abort = false;
                    return;
                }

                float[] vertex = null;
                lock (applyLocker)
                {
                    //already altered by another mesh build.
                    if (Vertex == null) return;

                    vertex = Vertex;
                    this.Vertex = null;
                }

                if (VertexArrayObject == -1)
                {
                    VertexBufferObject = GL.GenBuffer();
                    ElementBufferObject = GL.GenBuffer();
                    VertexArrayObject = GL.GenVertexArray();
                }

                GL.BindVertexArray(VertexArrayObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (int)Indices * sizeof(uint), this.Triangles, BufferUsageHint.StaticDraw);

                if (deleteWorkArrays)
                {
                    Triangles = null;
                }
                alreadyBuilding = false;
            });
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
            this.Indices = 0;

            Graphics.Window.InvokeRender(() =>
            {
                GL.DeleteBuffer(ElementBufferObject);
                GL.DeleteBuffer(VertexBufferObject);
                GL.DeleteVertexArray(VertexArrayObject);
            });


            OnDelete?.Invoke();
            OnDelete = null;

            Cache.Remove(this);

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
