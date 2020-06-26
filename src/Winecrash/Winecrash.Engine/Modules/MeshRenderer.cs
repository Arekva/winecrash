using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System.Windows.Forms;

namespace Winecrash.Engine
{
    public sealed class MeshRenderer : Module
    {
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }

        internal static List<MeshRenderer> ActiveMeshRenderers = new List<MeshRenderer>();

        private int PreviousFrameHash = 0;
        private bool RebuildBuffers = true;

        private int VertexBufferObject = -1;
        private int ElementBufferObject = -1;
        private int VertexArrayObject = -1;

        internal void Use(Matrix4 transform)
        {
            if (Deleted || Material == null) return;
            if (Mesh == null || Mesh.Vertices == null || Mesh.Triangles == null) return;

            if(VertexBufferObject == -1)
            {
                VertexBufferObject = GL.GenBuffer();
                
            }

            if(ElementBufferObject == -1)
            {
                ElementBufferObject = GL.GenBuffer();
                
            }

            if(VertexArrayObject == -1)
            {
                VertexArrayObject = GL.GenVertexArray();
                //GL.BindVertexArray(VertexArrayObject);
            }

            if (RebuildBuffers)
            {
                float[] vertex = Mesh.cube_vertices;//Mesh.Vertex;
                uint[] tris = Mesh._Indices;//Mesh.Triangles;

                GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, tris.Length * sizeof(uint), tris, BufferUsageHint.StaticDraw);
            }


            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferObject);
            
            GL.BindVertexArray(VertexArrayObject);

            Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
            Material.Shader.SetAttribute("uv", AttributeTypes.UV);
            Material.SetData<Matrix4>("transform", transform);
            Material.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, Mesh./*Vertices*/cube_vertices.Length / 5);
        }

        protected internal override void Creation()
        {
            ActiveMeshRenderers.Add(this);
            this.Group = -1;
        }
        protected internal override void Update()
        {
            if (!Mesh) return;

            int frameHash = Mesh.ModelHash;
            
            if(frameHash != PreviousFrameHash)
            {
                PreviousFrameHash = frameHash;
                RebuildBuffers = true;
            }
        }
        protected internal override void OnEnable()
        {
            ActiveMeshRenderers.Add(this);
        }

        protected internal override void OnDisable()
        {
            ActiveMeshRenderers.Remove(this);
        }

        protected internal override void OnDelete()
        {


            ActiveMeshRenderers.Remove(this);
            this.Mesh = null;
        }
    }
}
