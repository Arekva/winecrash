using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System.Windows.Forms;
using System.Diagnostics;

namespace Winecrash.Engine
{
    public sealed class MeshRenderer : Module
    {
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }

        internal static List<MeshRenderer> ActiveMeshRenderers = new List<MeshRenderer>();
        internal static List<MeshRenderer> MeshRenderers = new List<MeshRenderer>();

        private int VertexBufferObject = -1;
        private int ElementBufferObject = -1;
        private int VertexArrayObject = -1;

        
        internal void Use(Matrix4 transform)
        {
            if (Deleted || Material == null) return;
            if (Mesh == null || Mesh.Vertices == null || Mesh.Triangles == null) return;

            
            if (VertexBufferObject == -1)
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
            }

            
            float[] vertex = Mesh.Vertex;
            uint[] tris = Mesh.Indices;

            

            if (Mesh.AskedForApply)
            {
                Debug.Log("Rebuilding the buffers for " + this.WObject.Name);

                GL.BindVertexArray(VertexArrayObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex, BufferUsageHint.StaticDraw);
                
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, tris.Length * sizeof(uint), tris, BufferUsageHint.StaticDraw);

                Mesh.AskedForApply = false;
            }

            
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);

            

            Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
            Material.Shader.SetAttribute("uv", AttributeTypes.UV);
            Material.Shader.SetAttribute("normal", AttributeTypes.Normal);

            Material.SetData<Matrix4>("transform", transform);
            //Material.SetData<Vector3>("scale", this);

            

            Material.Use();
            GL.DepthMask(true);
            GL.DrawElements(PrimitiveType.Triangles, tris.Length * 4, DrawElementsType.UnsignedInt, 0);

        }

        protected internal override void Creation()
        {
            ActiveMeshRenderers.Add(this);
            MeshRenderers.Add(this);
            this.Group = -1;
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
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            MeshRenderers.Remove(this);
            ActiveMeshRenderers.Remove(this);
            this.Mesh = null;
        }
    }
}
