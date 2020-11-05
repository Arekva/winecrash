using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WEngine
{
    public delegate void MeshRenderDelegate();
    
    public class MeshRenderer : Module
    {
        public virtual Mesh Mesh { get; set; } = null;
        protected object MeshLocker = new object();

        public Material Material { get; set; } = Material.Find("Error");
        protected object MaterialLocker = new object();

        internal static object ActiveMeshRenderersLocker = new object();
        internal static List<MeshRenderer> ActiveMeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static List<MeshRenderer> MeshRenderers { get; set; } = new List<MeshRenderer>();
        internal static object MeshRenderersLocker { get; } = new object();
        public Culling Culling { get; set; } = Culling.Front;
        public bool UseMask { get; set; } = true;
        public bool UseDepth { get; set; } = true;
        public bool Wireframe { get; set; } = false;
        public int DrawOrder { get; set; } = 0;
        public static bool Global_Wireframe { get; set; } = false;

        protected bool CheckValidity(Camera sender)
        {
            return (!this.Enabled || (this.WObject.Layer & sender.RenderLayers) == 0) || MeshLocker == null || Deleted || Mesh == null || Mesh.ElementBufferObject == -1 || Mesh.VertexArrayObject == -1 || Mesh.VertexBufferObject == -1;
        }


        protected Vector3D _RenderPosition;
        protected Vector3D _RenderForward;
        protected Vector3D _RenderUp;
        protected Vector3D _RenderScale;
        protected Quaternion _RenderRotation;

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
                    
                    BindBuffers();
                    ComputeMatricesGPU(sender);
                    
                    this.Material.Use();

                    SetGLProperties();

                    DrawModel();
                }
            }
        }

        protected internal virtual void BindBuffers()
        {
            GL.BindVertexArray(Mesh.VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Mesh.VertexBufferObject);
        }

        protected internal virtual void DrawModel()
        {
            GL.DrawElements((Wireframe | Global_Wireframe) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)Mesh.Indices, DrawElementsType.UnsignedInt, 0);
        }

        protected internal virtual void ComputeMatricesCPU(Camera sender)
        {
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
            Matrix4D vmat = sender._RenderView;//new Matrix4D(Vector3D.Zero, sender._RenderForward, sender._RenderUp);

            Matrix4D.Mult(in objTrans, in vmat, out transform);
            Matrix4D.Mult(in transform, sender._RenderProjectionMatrix, out finalTransform);
            
            this.Material.SetData("transform", finalTransform);
            this.Material.SetData("rotation", _RenderRotation);
        }

        protected internal virtual void SetGLProperties()
        {
            if(UseDepth)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(UseMask);
                    
            GL.CullFace((CullFaceMode)Culling);
        }

        protected internal virtual void ComputeMatricesGPU(Camera sender)
        {
            Matrix4D objTrans;
            Matrix4D scaD = new Matrix4D(_RenderScale, true);
            Matrix4D rotD = new Matrix4D(_RenderRotation);
            Matrix4D posD = new Matrix4D(_RenderPosition - sender._RenderPosition, false);
            Matrix4D.Mult(in scaD, in rotD, out objTrans);
            Matrix4D.Mult(in objTrans, in posD, out objTrans);
            Matrix4D.Mult(in objTrans, Matrix4D.Identity, out objTrans);
            
            this.Material.SetData("model", objTrans);
            this.Material.SetData("view", sender._RenderView);
            this.Material.SetData("projection", sender._RenderProjectionMatrix);
            this.Material.SetData("rotation", _RenderRotation);
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
