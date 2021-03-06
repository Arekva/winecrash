﻿using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK.Graphics.OpenGL4;

namespace WEngine
{
    public sealed class Camera : Module
    {
        internal static List<Camera> Cameras { get; set; } = new List<Camera>(1);

        public static Camera Main { get; set; }

        public UInt64 RenderLayers { get; set; } = UInt64.MaxValue;
        
        public double Depth { get; set; } = 0.0D;


        public CameraProjectionType ProjectionType { get; set; } = CameraProjectionType.Perspective;

        public Vector2D OrthographicSize { get; set; } = Vector2D.One;

        /// <summary>
        /// -Vertical- Field of View
        /// </summary>
        public double FOV { get; set; } = 45.0D;


        public Vector2I Resolution
        {
            get
            {
                return Graphics.Window.SurfaceResolution;//Viewport.Instance == null ? new Vector2I(1024, 1024) : new Vector2I(Viewport.Instance.ClientSize.Width, Viewport.Instance.ClientSize.Height);
            }
        }

        public double AspectRatio
        {
            get
            {
                Vector2I res = this.Resolution;
                return (double)res.X / (double)res.Y;
            }
        }

        public Vector2D ClipPlanes
        {
            get
            {
                return new Vector2D(this._NearClip, this._FarClip);
            }
        }

        public double _NearClip = 0.1D;
        public double NearClip
        {
            get
            {
                return this._NearClip;
            }

            set
            {
                this._NearClip = WMath.Clamp(value, 0.01D, this._FarClip - 0.01D);
            }
        }
        public double _FarClip = 100.0D;
        public double FarClip
        {
            get
            {
                return this._FarClip;
            }

            set
            {
                this._FarClip = WMath.Clamp(value, this._NearClip + 0.01D, 10000.0D);
            }
        }

        private Color256 _ClearColor = new Color256(1.0, 1.0, 1.0, 1.0);
        public Color256 ClearColor
        {
            get
            {
                return this._ClearColor;
            }

            set
            {
                Color256 col = this._ClearColor = value;
                Graphics.Window.InvokeRender(() => GL.ClearColor((float)col.R, (float)col.G, (float)col.B, 255));
            }
        }

        //internal Matrix4D _RenderViewMatrix;
        internal Matrix4D ViewMatrix
        {
            get
            {
                Vector3D p = this.WObject.Position;
                Vector3D t = this.WObject.Forward;
                Vector3D u = this.WObject.Up;

                //ok
                return new Matrix4D(new Vector3D(p.X, p.Y, p.Z), new Vector3D(p.X + t.X, p.Y + t.Y, p.Z + t.Z), new Vector3D(u.X, u.Y, u.Z));
            }
        }

        internal Matrix4D ViewMatrixRelative
        {
            get
            {
                Vector3D p = this.WObject.Position;
                Vector3D t = this.WObject.Forward;
                Vector3D u = this.WObject.Up;

                //ok
                return new Matrix4D(Vector3D.Zero, /*new Vector3D(p.X + t.X, p.Y + t.Y, p.Z + t.Z)*/t, /*new Vector3D(u.X, u.Y, u.Z)*/u);
            }
        }

        internal void ViewMatrixRef(out Matrix4D result)
        {
            Vector3D p = this.WObject.Position;
            Vector3D t = this.WObject.Forward;
            Vector3D u = this.WObject.Up;

            Matrix4D.LookAtRef(new Vector3D(p.X, p.Y, p.Z), new Vector3D(p.X + t.X, p.Y + t.Y, p.Z + t.Z), new Vector3D(u.X, u.Y, u.Z), out result);
        }
        //internal Matrix4D _RenderProjectionMatrix;
        internal Matrix4D ProjectionMatrix
        {
            get
            {
                // ok
                return this.ProjectionType == CameraProjectionType.Perspective ? 
                    new Matrix4D(this.FOV * WMath.DegToRad, this.AspectRatio, this._NearClip, this._FarClip):
                    Matrix4D.Orthographic(OrthographicSize.X, OrthographicSize.Y, this._NearClip, this._FarClip);
            }
        }


        protected internal override void OnEnable()
        {
            Cameras.Add(this);
        }

        protected internal override void OnDisable()
        {
            Cameras.Remove(this);
        }

        protected internal override void OnDelete()
        {
            Cameras.Remove(this);
            if(Main != null && Main == this)
            {
                Main = null;
            }
        }



        internal Vector3D _RenderPosition;
        internal Vector3D _RenderScale;
        internal Quaternion _RenderRotation;
        internal Matrix4D _RenderProjectionMatrix;
        internal Vector3D _RenderUp;
        internal Vector3D _RenderForward;
        internal Matrix4D _RenderView;
        internal Matrix4D _worldRenderView;

        internal void PrepareForRender()
        {
            _RenderPosition = this.WObject.Position;
            _RenderScale = this.WObject.Scale;
            _RenderRotation = this.WObject.Rotation;
            _RenderProjectionMatrix = this.ProjectionMatrix;
            _RenderUp = this.WObject.Up;
            _RenderForward = this.WObject.Forward;
            _RenderView = new Matrix4D(Vector3D.Zero, this._RenderForward, this._RenderUp);
            _worldRenderView = new Matrix4D(-this._RenderPosition, Vector3D.Forward, Vector3D.Up);
        }

        protected internal override void OnRender()
        {
            if (!this.Enabled) return;

            List<Material> mats = null;
            lock (Material.CacheLocker)
                mats = Material.Cache.ToList();

            foreach (Material mat in mats.OrderBy(m => m.RenderOrder))
                mat.Draw(this);

                /*MeshRenderer[] mrs = null;
                lock(MeshRenderer.ActiveMeshRenderersLocker)
                    mrs = MeshRenderer.ActiveMeshRenderers.ToArray();
                
                mrs = mrs.Where(mr => mr != null && mr.WObject != null && (mr.WObject.Layer & this.RenderLayers) != 0).OrderBy(mr => mr.DrawOrder).ToArray();
    */
            //for (int i = 0; i < mrs.Length; i++)
            //    mrs[i].Use(this);
        }
    }
}
