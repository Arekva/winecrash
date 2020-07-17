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
    public delegate void MeshRenderDelegate();
    
    public class MeshRenderer : Module
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
        protected Mesh _Mesh = null;

        public Material Material { get; set; }

        internal static List<MeshRenderer> ActiveMeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static List<MeshRenderer> MeshRenderers { get; set; } = new List<MeshRenderer>();

        public event MeshRenderDelegate OnRender;

        public bool UseMask { get; set; } = true;
        public bool Wireframe { get; set; } = false;

        internal virtual void Use(Camera sender)
        {
            if (Deleted || Material == null || _Mesh == null || _Mesh.Indices == null || _Mesh.ElementBufferObject == -1 || _Mesh.VertexArrayObject == -1 || _Mesh.VertexBufferObject == -1) return;

            Matrix4 transform = this.WObject.TransformMatrix * sender.ViewMatrix * sender.ProjectionMatrix;

            GL.BindVertexArray(_Mesh.VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _Mesh.VertexBufferObject);

            this.Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
            this.Material.Shader.SetAttribute("uv", AttributeTypes.UV);
            this.Material.Shader.SetAttribute("normal", AttributeTypes.Normal);

            this.Material.SetData<Matrix4>("transform", transform);

            this.Material.Use();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(UseMask);

            OnRender?.Invoke();

            GL.DrawElements(Wireframe ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)_Mesh.Indices, DrawElementsType.UnsignedInt, 0);
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
            MeshRenderers.Remove(this);
            ActiveMeshRenderers.Remove(this);
            this._Mesh = null;
        }
    }
}
