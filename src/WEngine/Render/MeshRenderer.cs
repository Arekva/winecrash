using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using WEngine.GUI;

namespace WEngine
{
    public delegate void MeshRenderDelegate();
    
    public class MeshRenderer : Module
    {
        public virtual Mesh Mesh { get; set; } = null;
        protected object MeshLocker = new object();

        private Material _material = Material.Find("Error");

        public Material Material
        {
            get => _material;
            
            set
            {
                if(_material)
                    _material.RenderersRemove(this);

                if(value) value.RenderersAdd(this);
                
                _material = value;
            }
        }
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
        public bool IgnoreDepthSort { get; set; } = false;
        public static bool Global_Wireframe { get; set; } = false;

        protected bool CheckValidity(Camera sender)
        {
            return (!this.Enabled || (this.WObject.Layer & sender.RenderLayers) == 0) || MeshLocker == null || Deleted || Mesh == null || Mesh.ElementBufferObject == -1 || Mesh.VertexArrayObject == -1 || Mesh.VertexBufferObject == -1;
        }


        protected Vector3D _renderPosition;
        protected Vector3D _renderForward;
        protected Vector3D _renderUp;
        protected Vector3D _renderScale;
        protected Quaternion _renderRotation;
        public Dictionary<Camera, double> CamerasDistances { get; private set; } = new Dictionary<Camera, double>();
        private object _camerasDistanceLocker = new object();
        
        internal virtual void PrepareForRender()
        {
            _renderPosition = this.WObject.Position;
            _renderForward = this.WObject.Forward;
            _renderScale = this.WObject.Scale;
            _renderRotation = this.WObject.Rotation;
            _renderUp = this.WObject.Up;

            ComputeDistances();
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

        protected virtual void ComputeDistances()
        {
            CamerasDistances.Clear();

            foreach (Camera camera in Camera.Cameras)
            {
                if ((camera.RenderLayers & this.WObject.Layer) != 0)
                {
                    if (camera.ProjectionType == CameraProjectionType.Perspective)
                    {
                        //todo: better occulusion

                        double dist = Vector3D.SquaredDistance(camera._RenderPosition, _renderPosition);
                        /*Vector3D camFwd = camera._RenderForward;
                        Vector3D relPos = _renderPosition - camera._RenderPosition;
                        double angle = Vector3D.SignedAngle(camFwd, relPos.Normalized,
                            camera._RenderUp);
                        //if object is behind camera, don't draw.
                        if (dist < 100.0D || angle > -45)*/
                        CamerasDistances.Add(camera, dist);
                    }
                    else
                    {
                        CamerasDistances.Add(camera, 0);
                    }
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
            Matrix4D scaD = new Matrix4D(_renderScale, true);
            Matrix4D rotD = new Matrix4D(_renderRotation);
            Matrix4D posD = new Matrix4D(_renderPosition - sender._RenderPosition, false);
            Matrix4D.Mult(in scaD, in rotD, out objTrans);
            Matrix4D.Mult(in objTrans, in posD, out objTrans);
            Matrix4D.Mult(in objTrans, Matrix4D.Identity, out objTrans);

            // camera view relative to object
            Matrix4D vmat = sender._RenderView;//new Matrix4D(Vector3D.Zero, sender._RenderForward, sender._RenderUp);

            Matrix4D.Mult(in objTrans, in vmat, out transform);
            Matrix4D.Mult(in transform, sender._RenderProjectionMatrix, out finalTransform);
            
            this.Material.SetData("transform", finalTransform);
            this.Material.SetData("rotation", _renderRotation);
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
            Matrix4D scaD = new Matrix4D(_renderScale, true);
            Matrix4D rotD = new Matrix4D(_renderRotation);
            Matrix4D posD = new Matrix4D(_renderPosition - sender._RenderPosition, false);
            Matrix4D.Mult(in scaD, in rotD, out objTrans);
            Matrix4D.Mult(in objTrans, in posD, out objTrans);
            Matrix4D.Mult(in objTrans, Matrix4D.Identity, out objTrans);
            
            this.Material.SetData("model", objTrans);
            this.Material.SetData("view", sender._RenderView);
            this.Material.SetData("projection", sender._RenderProjectionMatrix);
            this.Material.SetData("rotation", _renderRotation);
        }

        protected internal override void Creation()
        {
            lock(MeshRenderersLocker) MeshRenderers.Add(this);
            this.Group = -1;
        }

        protected internal override void OnEnable()
        {
            lock(ActiveMeshRenderersLocker) 
                if(!ActiveMeshRenderers.Contains(this)) ActiveMeshRenderers.Add(this);
        }

        protected internal override void OnDisable()
        {
            lock (ActiveMeshRenderersLocker) ActiveMeshRenderers.Remove(this);
        }

        protected internal override void OnDelete()
        {
            lock (ActiveMeshRenderersLocker) ActiveMeshRenderers.Remove(this);

            lock (MeshRenderersLocker) MeshRenderers.Remove(this);
            
                        
            lock (MaterialLocker)
                this.Material = null;

            lock(MeshLocker)
                this.Mesh = null;

            MeshLocker = null;
            MaterialLocker = null;
        }
    }
}
