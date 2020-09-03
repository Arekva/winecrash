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

        public Material Material { get; set; } = Material.Find("Error");

        internal static List<MeshRenderer> ActiveMeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static List<MeshRenderer> MeshRenderers { get; set; } = new List<MeshRenderer>();

        //public event MeshRenderDelegate OnRender;

        public bool UseMask { get; set; } = true;
        public bool Wireframe { get; set; } = false;

        public static bool Global_Wireframe { get; set; } = false;

        protected bool CheckValidity(Camera sender)
        {
            return (!this.Enabled || (this.WObject.Layer & sender.RenderLayers) == 0) || Deleted || _Mesh == null  || _Mesh.ElementBufferObject == -1 || _Mesh.VertexArrayObject == -1 || _Mesh.VertexBufferObject == -1;
        }

        internal virtual void Use(Camera sender)
        {
            if (CheckValidity(sender)) return;

            Matrix4D transform;
            //sender.ViewMatrixRef(out Matrix4D view);
            this.WObject.TransformMatrixRef(out Matrix4D trans);
            Matrix4D.Mult(in trans, in sender._RenderViewMatrix, out transform);
            Matrix4D.Mult(in transform, sender._RenderProjectionMatrix, out transform);

            GL.BindVertexArray(_Mesh.VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _Mesh.VertexBufferObject);

            this.Material.SetData<Matrix4>("transform", (Matrix4)transform);

            this.Material.Use();


            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(UseMask);

            //OnRender?.Invoke();

            GL.DrawElements((Wireframe | Global_Wireframe) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)_Mesh.Indices, DrawElementsType.UnsignedInt, 0);
        }

        protected internal override void Creation()
        {
            lock(ActiveMeshRenderers)
            {
                MeshRenderers.Add(this);
            }
            this.Group = -1;
        }

        protected internal override void OnEnable()
        {
            object obj = new object();
            lock(obj)
            {
                ActiveMeshRenderers.Add(this);
            }
        }

        protected internal override void OnDisable()
        {
            object obj = new object();
            lock (obj)
            {
                ActiveMeshRenderers.Remove(this);
            }
        }

        protected internal override void OnDelete()
        {
            object obj = new object();
            lock(obj)
            {
                Material = null;
                MeshRenderers.Remove(this);
                ActiveMeshRenderers.Remove(this);
            }
            this._Mesh = null;
        }
    }
}
