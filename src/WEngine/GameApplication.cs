using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NAudio.Dsp;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace WEngine
{
    /// <summary>
    /// The game application is a all-in-one window displaying the current scenes.
    /// </summary>
    public class GameApplication : GameWindow, IWindow
    {
        //      IWindow.SurfaceResolution        //
        public Vector2I SurfaceFixedResolution
        {
            get
            {
                return _SurfaceFixedResolution;
            }
            set
            {
                if (_SurfaceFixedResolution != value)
                {
                    _SurfaceFixedResolution = value;
                    _SurfaceFixedResolutionChanged = true;
                }
            }
        }
        private Vector2I _SurfaceFixedResolution = new Vector2I(1024, 720);
        private bool _SurfaceFixedResolutionChanged = true;

        public Vector2I SurfaceResolution
        {
            get
            {
                Size s = base.ClientSize;
                return new Vector2I(s.Width, s.Height);
            }
        }

        public Vector2I SurfacePosition
        {
            get
            {
                Size s = base.Size;
                Size cs = base.ClientSize;

                Vector2I deltaSize = new Vector2I(s.Width, s.Height) - new Vector2I(cs.Width, cs.Height);
                Point p = base.Location;

                return new Vector2I(p.X, p.Y) + deltaSize;
            }

            set
            {
                Size s = base.Size;
                Size cs = base.ClientSize;
                Vector2I deltaSize = new Vector2I(s.Width, s.Height) - new Vector2I(cs.Width, cs.Height);
                Point p = new Point(value.X, value.Y);

                Vector2I final = value + deltaSize;

                this.Location = new Point(final.X, final.Y);
            }
        }

        public new string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if(value != _Title)
                {
                    _Title = value;
                    _TitleChanged = true;
                }
            }
        }
        private string _Title = "Game Application";
        private bool _TitleChanged = true;

        //          IWindow.WindowState          //
        public new WindowState WindowState
        {
            get
            {
                return _WindowState;
            }
            set
            {
                if (value != _WindowState)
                {
                    _WindowState = value;
                    _WindowStateChanged = true;
                }
            }
        }
        private WindowState _WindowState = WEngine.WindowState.Normal;
        private bool _WindowStateChanged = true;

        //             IWindow.VSync             //
        public new VSyncMode VSync
        {
            get
            {
                return _VSync;
            }
            set
            {
                if (value != _VSync)
                {
                    _VSync = value;
                    _VSyncChanged = true;
                }
            }
        }
        private VSyncMode _VSync = VSyncMode.On;
        private bool _VSyncChanged = true;

        //           IWindow.Resizable           //
        public bool Resizable
        {
            get
            {
                return _Resizable;
            }
            set
            {
                if(_Resizable != value)
                {
                    _Resizable = value;
                    _ResizableChanged = true;
                }
            }
        }
        private bool _Resizable = true;
        private bool _ResizableChanged = true;

        //            IWindow.Focused           //
        public new bool Focused
        {
            get
            {
                return base.IsDisposed ? false : base.Focused;
            }
        }
        

        //         IWindow.CursorVisible        //
        public new bool CursorVisible
        {
            get
            {
                return _CursorVisible;
            }
            set
            {
                if (_CursorVisible != value)
                {
                    _CursorVisible = value;
                    _CursorVisibleChanged = true;
                }
            }
        }
        private bool _CursorVisible = true;
        private bool _CursorVisibleChanged = true;

        public new event EventHandler<EventArgs> OnResizing;

        public event WindowInvokeDelegate OnLoaded;
        public event UpdateEventHandler OnUpdate;   
        public event UpdateEventHandler OnRender;

        public Thread Thread { get; set; }

        private MouseState _PreviousMouseState = new MouseState();

        private Action _InvokeOnRender;
        private object _InvokeRenderLocker = new object();

        private Action _InvokeOnUpdate;
        private object _InvokeUpdateLocker = new object();

        public void InvokeUpdate(Action action)
        {
            lock (_InvokeUpdateLocker)
            {
                _InvokeOnUpdate += action;
            }
        }
        public void InvokeRender(Action action)
        {
            lock (_InvokeRenderLocker)
            {
                _InvokeOnRender += action;
            }
        }

        public GameApplication(string title, Icon icon = null) : base()
        {
            this.Title = title;
            this.Icon = icon;
        }

        /// <summary>
        /// Initiliaze all the basis to make the game run.
        /// </summary>
        protected void InitializeGame()
        {
            // create the main game camera
            WObject camWobj = new WObject("Main Camera");
            Camera cam = camWobj.AddModule<Camera>();
            Camera.Main = cam;

            Shader.CreateError(); // el famoso pinko del shader

            GL.FrontFace(FrontFaceDirection.Cw); // set front to counter wise
            GL.CullFace(CullFaceMode.Front); // set cull to front
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.DepthTest); // enable depth test
            GL.DepthFunc(DepthFunction.Lequal); // set anything less or equal to current depth being drawn onto screen
            
            GL.Enable(EnableCap.Blend); // transparency. further modes are available into shaders. or material, I don't remember

            // create basic shaders. if visual studio yells at you while underlining in green,
            // don't listen to it it's dumb, everything is stored into the shader cache.
            new Shader("assets/shaders/Standard/Standard.vert", "assets/shaders/Standard/Standard.frag");
            new Shader("assets/shaders/Unlit/Unlit.vert", "assets/shaders/Unlit/Unlit.frag");
            new Shader("assets/shaders/Text/Text.vert", "assets/shaders/Text/Text.frag");

            new GUI.Font("assets/fonts/pixelized.json", "Pixelized");

        }

        public Bitmap Screenshot()
        {
            Process proc = Process.GetCurrentProcess();

            Bitmap bmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);

            Rectangle winBounds = this.Bounds;
            Rectangle rect = ClientRectangle;

            Vector2I shift = Vector2I.Zero;

            if (Engine.OS.Platform == OSPlatform.Windows && this.WindowState == WindowState.Normal)
            {
                shift -= new Vector2I(8, 8);
            }

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bmp))
            {
                graphics.CopyFromScreen(SurfacePosition.X + shift.X, SurfacePosition.Y + shift.Y, 0, 0, new Size(SurfaceResolution.X, SurfaceResolution.Y), CopyPixelOperation.SourceCopy);
            }

            return bmp;
        }

        public Vector2I ScreenToWindow(Vector2I point)
        {
            Point p = new Point(point.X, point.Y);
            p = base.PointToClient(p);

            Vector2I uncorrect = new Vector2I(p.X, p.Y);

            uncorrect.X = (int)((float)this.SurfaceResolution.X * 0.5) - uncorrect.X;
            uncorrect.Y = (int)((float)this.SurfaceResolution.Y * 0.5) - uncorrect.Y;

            return uncorrect;
        }

        public new void Close()
        {
            base.Close();
        }

        WindowBorder _PreviousBorder;
        
        
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if(_TitleChanged)
            {
                _TitleChanged = false;
                base.Title = _Title;
            }
            if(_VSyncChanged)
            {
                _VSyncChanged = false;
                base.VSync = (OpenTK.VSyncMode)this._VSync;
            }
            if(_SurfaceFixedResolutionChanged)
            {
                _SurfaceFixedResolutionChanged = false;
                if(_Resizable) base.ClientSize = new Size(_SurfaceFixedResolution.X, _SurfaceFixedResolution.Y);
            }
            if(_WindowStateChanged)
            {
                _WindowStateChanged = false;
                base.WindowState = (OpenTK.WindowState)this.WindowState; 
            }
            if(_ResizableChanged)
            {
                _ResizableChanged = false;
                base.WindowBorder = _Resizable ? WindowBorder.Resizable : WindowBorder.Fixed;

                if(!_Resizable) base.ClientSize = new Size(_SurfaceFixedResolution.X, _SurfaceFixedResolution.Y);
            }
            if(_CursorVisibleChanged)
            {
                _CursorVisibleChanged = false;
                //base.CursorVisible = _CursorVisible;
            }

            if(_PreviousBorder != base.WindowBorder)
            {
                _PreviousBorder = base.WindowBorder;
                OnResizing?.Invoke(this, new EventArgs());
            }

            // if not focused cursor always visible.
            base.CursorVisible = base.Focused ? _CursorVisible : true;


            Action doUpdateOnce;
            lock(_InvokeUpdateLocker)
            {
                doUpdateOnce = _InvokeOnUpdate;
                _InvokeOnUpdate = null;
            }
            doUpdateOnce?.Invoke();

            MouseState state = Mouse.GetState();
            Input.SetMouseScroll(state.WheelPrecise, state.Wheel);



            Time.FrameTimer.Stop();
            Time.DeltaTime = Time.FrameTimer.Elapsed.TotalSeconds * Time.TimeScale;//e.Time * Time.TimeScale; 
            
            Time.FrameTimer.Reset();
            Time.FrameTimer.Start();
            
            
            //Debug.Log(Time.DeltaTime);

            MouseState ms = Mouse.GetState();
            Vector2D delta = new Vector2D(this._PreviousMouseState.X - ms.X, this._PreviousMouseState.Y - ms.Y);
            this._PreviousMouseState = ms;

            OnUpdate?.Invoke(new UpdateEventArgs(e.Time));

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //todo clear: make the camera clear by themselves if needed.
            GL.ClearDepth(1.0D);

            Action onRenderOnce;
            lock (_InvokeRenderLocker)
            {
                onRenderOnce = _InvokeOnRender;
                _InvokeOnRender = null;
            }
            onRenderOnce?.Invoke();
            
            
            Layer.RenderThreadLocker.WaitOne();
            Layer.PhysicsThreadLocker.Reset();
            
            
            lock(MeshRenderer.ActiveMeshRenderersLocker)
            {
                Parallel.ForEach(MeshRenderer.ActiveMeshRenderers, mr =>
                {
                    mr.PrepareForRender();
                });
            }

            Layer.PhysicsThreadLocker.Set();


            Time.ResetRender();
            Time.BeginRender();
            OnRender?.Invoke(new UpdateEventArgs(e.Time));
            Time.EndRender();
            

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            InitializeGame();

            OnLoaded?.Invoke();

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            Mesh[] meshes = Mesh.Cache.ToArray();
            if (meshes != null)
                for (int i = 0; i < meshes.Length; i++)
                    meshes[i]?.Delete();


            Shader[] shaders = Shader.Cache.ToArray();
            if (shaders != null)
                for (int i = 0; i < shaders.Length; i++)
                    shaders[i]?.Delete();

            Texture[] textures;
            lock(Texture.CacheLocker)
                textures = Texture.Cache.ToArray();

            if (textures != null)
                for (int i = 0; i < textures.Length; i++)
                    textures[i]?.Delete();

            base.OnUnload(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Engine.Stop(this);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            OnResizing?.Invoke(this, e);
            base.OnResize(e);
        }
        protected override void OnWindowStateChanged(EventArgs e)
        {
            OnResizing?.Invoke(this, e);
            base.OnWindowStateChanged(e);
        }
        protected override void OnWindowBorderChanged(EventArgs e)
        {
            OnResizing?.Invoke(this, e);
            base.OnWindowBorderChanged(e);
        }
    }
}
