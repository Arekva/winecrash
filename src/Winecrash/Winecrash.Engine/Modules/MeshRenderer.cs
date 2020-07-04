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
        public Mesh Mesh
        {
            get
            { 
                return this._Mesh;
            }

            set
            {
                this._Mesh = value;
            }
        }
        private Mesh _Mesh = null;

        public Material Material { get; set; }

        internal static List<MeshRenderer> ActiveMeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static List<MeshRenderer> MeshRenderers { get; set; } = new List<MeshRenderer>();

        private int VertexBufferObject = -1;
        private int ElementBufferObject = -1;
        private int VertexArrayObject = -1;

        public bool UseMask { get; set; } = true;

        private Mesh previousMesh = null;
        internal void Use(Camera sender)
        {
            if (Deleted || Material == null || _Mesh == null) return;

            Matrix4 transform = this.WObject.TransformMatrix * sender.ViewMatrix * sender.ProjectionMatrix;
            
            //Debug.Log(transform);

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

            
            float[] vertex = _Mesh.Vertex;
            uint[] tris = _Mesh.Indices;

            if (previousMesh != this._Mesh || _Mesh.AskedForApply)
            {
                GL.BindVertexArray(VertexArrayObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex, BufferUsageHint.StaticDraw);
                
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, tris.Length * sizeof(uint), tris, BufferUsageHint.StaticDraw);

                _Mesh.AskedForApply = false;
            }

            
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            

            this.Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
            this.Material.Shader.SetAttribute("uv", AttributeTypes.UV);
            this.Material.Shader.SetAttribute("normal", AttributeTypes.Normal);

            this.Material.SetData<Matrix4>("transform", transform);
            //Material.SetData<Vector3>("scale", this);

            

            this.Material.Use();
            GL.DepthMask(UseMask);

            if (_Mesh == null || _Mesh.Deleted || tris == null || VertexArrayObject == -1 || VertexBufferObject == -1) return;
            GL.DrawElements(PrimitiveType.Triangles, tris.Length, DrawElementsType.UnsignedInt, 0);

            previousMesh = this._Mesh;
        }

        protected internal override void Creation()
        {
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
            Viewport.DoOnce += () =>
            {
                GL.DeleteBuffer(VertexBufferObject);
                GL.DeleteVertexArray(VertexArrayObject);
            };

            MeshRenderers.Remove(this);
            ActiveMeshRenderers.Remove(this);
            this._Mesh = null;
        }
    }
}
