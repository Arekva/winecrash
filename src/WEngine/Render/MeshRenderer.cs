using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WEngine
{
    public delegate void MeshRenderDelegate();
    
    public class MeshRenderer : Module
    {
        public Mesh Mesh { get; set; } = null;
        private object MeshLocker = new object();

        public Material Material { get; set; } = Material.Find("Error");
        private object MaterialLocker = new object();

        internal static object ActiveMeshRenderersLocker = new object();
        internal static List<MeshRenderer> ActiveMeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static List<MeshRenderer> MeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static object MeshRenderersLocker { get; } = new object();
        
        public bool UseMask { get; set; } = true;
        public bool UseDepth { get; set; } = true;
        public bool Wireframe { get; set; } = false;

        public int DrawOrder { get; set; } = 0;

        public static bool Global_Wireframe { get; set; } = false;

        protected bool CheckValidity(Camera sender)
        {
            return (!this.Enabled || (this.WObject.Layer & sender.RenderLayers) == 0) || MeshLocker == null || Deleted || Mesh == null || Mesh.ElementBufferObject == -1 || Mesh.VertexArrayObject == -1 || Mesh.VertexBufferObject == -1;
        }


        private Vector3D _RenderPosition;
        private Vector3D _RenderForward;
        private Vector3D _RenderUp;
        private Vector3D _RenderScale;
        private Quaternion _RenderRotation;

        internal virtual void PrepareForRender()
        {
            _RenderPosition = this.WObject.Position;
            _RenderForward = this.WObject.Forward;
            _RenderScale = this.WObject.Scale;
            _RenderRotation = this.WObject.Rotation;
            _RenderUp = this.WObject.Up;
        }

        internal virtual void Use(Camera sender)
        {
            if (MeshLocker == null || MaterialLocker == null) return;

            lock (MaterialLocker)
            {
                lock (MeshLocker)
                {
                    if (CheckValidity(sender)) return;
                    

                    Matrix4D transform;
                    Matrix4D objTrans;
                    Matrix4D finalTransform;

                    // object transform
                    Matrix4D scaD = new Matrix4D(_RenderScale, true);
                    Matrix4D rotD = new Matrix4D(_RenderRotation);
                    Matrix4D posD = new Matrix4D(_RenderPosition - sender._RenderPosition, false);
                    Matrix4D.Mult(in scaD, in rotD, out objTrans);
                    Matrix4D.Mult(in objTrans, in posD, out objTrans);
                    Matrix4D.Mult(in objTrans, Matrix4D.Identity, out objTrans);

                    // camera view relative to object
                    Matrix4D vmat = new Matrix4D(Vector3D.Zero, sender._RenderForward, sender._RenderUp);

                    Matrix4D.Mult(in objTrans, in vmat, out transform);
                    Matrix4D.Mult(in transform, sender._RenderProjectionMatrix, out finalTransform);

                    GL.BindVertexArray(Mesh.VertexArrayObject);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, Mesh.VertexBufferObject);

                    this.Material.SetData<Matrix4>("transform", (Matrix4)finalTransform);

                    this.Material.Use();


                    if(UseDepth)
                    GL.Enable(EnableCap.DepthTest);
                    else
                    GL.Disable(EnableCap.DepthTest);
                    GL.DepthMask(UseMask);

                    //OnRender?.Invoke();

                    GL.DrawElements((Wireframe | Global_Wireframe) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)Mesh.Indices, DrawElementsType.UnsignedInt, 0);
                }
            }
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
            
                        
            lock (MaterialLocker)
                this.Material = null;

            lock(MeshLocker)
                this.Mesh = null;

            MeshLocker = null;
            MaterialLocker = null;
        }
    }
}
