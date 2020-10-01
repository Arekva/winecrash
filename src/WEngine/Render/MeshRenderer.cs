using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WEngine
{
    public delegate void MeshRenderDelegate();
    
    public class MeshRenderer : Module
    {
        public Mesh Mesh { get; set; } = null;

        public Material Material { get; set; } = Material.Find("Error");

        internal static object ActiveMeshRenderersLocker = new object();
        internal static List<MeshRenderer> ActiveMeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static List<MeshRenderer> MeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static object MeshRenderersLocker { get; } = new object();
        
        public bool UseMask { get; set; } = true;
        public bool Wireframe { get; set; } = false;

        public static bool Global_Wireframe { get; set; } = false;

        protected bool CheckValidity(Camera sender)
        {
            return (!this.Enabled || (this.WObject.Layer & sender.RenderLayers) == 0) || Deleted || Mesh == null  || Mesh.ElementBufferObject == -1 || Mesh.VertexArrayObject == -1 || Mesh.VertexBufferObject == -1;
        }

        internal virtual void Use(Camera sender)
        {
            if (CheckValidity(sender)) return;

            Matrix4D transform;
            //sender.ViewMatrixRef(out Matrix4D view);
            this.WObject.TransformMatrixRef(out Matrix4D trans);
            Matrix4D.Mult(in trans, in sender._RenderViewMatrix, out transform);
            Matrix4D.Mult(in transform, sender._RenderProjectionMatrix, out transform);

            GL.BindVertexArray(Mesh.VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Mesh.VertexBufferObject);

            this.Material.SetData<Matrix4>("transform", (Matrix4)transform);

            this.Material.Use();


            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(UseMask);

            //OnRender?.Invoke();

            GL.DrawElements((Wireframe | Global_Wireframe) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)Mesh.Indices, DrawElementsType.UnsignedInt, 0);
        }

        protected internal override void Creation()
        {
            lock(ActiveMeshRenderersLocker)
            {
                MeshRenderers.Add(this);
            }
            this.Group = -1;
        }

        protected internal override void OnEnable()
        {
            lock(ActiveMeshRenderersLocker)
            {
                ActiveMeshRenderers.Add(this);
            }
        }

        protected internal override void OnDisable()
        {
            lock (ActiveMeshRenderersLocker)
            {
                ActiveMeshRenderers.Remove(this);
            }
        }

        protected internal override void OnDelete()
        {
            lock (ActiveMeshRenderersLocker)
                ActiveMeshRenderers.Remove(this);

            lock (MeshRenderersLocker)
                MeshRenderers.Remove(this);
            
                        
            this.Material = null;
            this.Mesh = null;
        }
    }
}
