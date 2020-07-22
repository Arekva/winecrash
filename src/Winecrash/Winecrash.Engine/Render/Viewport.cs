using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Diagnostics;
using System.Drawing;

namespace Winecrash.Engine
{
    public class Viewport : GameWindow
    {
        public static Viewport Instance { get; private set; }

        internal static Thread ThreadRunner { get; set; }

        public static event UpdateEventHandler Update;
        public static event UpdateEventHandler Render;
        public static event ViewportDoOnceDelegate DoOnce;
        public static event ViewportDoOnceDelegate DoOnceRender;

        public static event ViewportLoadDelegate OnLoaded;

        public delegate void ViewportLoadDelegate();
        public delegate void ViewportDoOnceDelegate();

        MouseState _PreviousState = new MouseState();

        /// <summary>
        /// https://github.com/opentk/LearnOpenTK/blob/master/Chapter1/4-Textures/Window.cs
        /// </summary>
        public Viewport(int width, int height, string title, Icon icon = null) : base(width, height, GraphicsMode.Default, title) 
        {
            if(icon != null)
            {
                this.Icon = icon;
            }
            Instance = this;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.VSync = VSyncMode.Off;

            new Texture();
            WObject camWobj = new WObject("Main Camera");
            Camera cam = camWobj.AddModule<Camera>();
            Camera.Main = cam;

            Shader.CreateError();

            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            new Shader("assets/shaders/Standard/Standard.vert", "assets/shaders/Standard/Standard.frag");
            new Shader("assets/shaders/Unlit/Unlit.vert", "assets/shaders/Unlit/Unlit.frag");
            new Shader("assets/shaders/Text/Text.vert", "assets/shaders/Text/Text.frag");

            OnLoaded?.Invoke();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearDepth(1.0D);


            object obj = new object();

            ViewportDoOnceDelegate once = DoOnceRender;
            once?.Invoke();
            DoOnceRender -= once;

            //updatesw.Start();
            Time.ResetRender();
            Time.BeginRender();
            Render?.Invoke(new UpdateEventArgs(e.Time));
            Time.EndRender();
            

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            /*GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);*/

            //GL.DeleteBuffer(_VertexBufferObject);
            //GL.DeleteVertexArray(_VertexArrayObject);

            MeshRenderer[] renderers = MeshRenderer.MeshRenderers.ToArray();
            if (renderers != null)
                for (int i = 0; i < renderers.Length; i++)
                    if (renderers[i] != null)
                        renderers[i].Delete();

            
            Shader[] shaders = Shader.Cache.ToArray();
            if(shaders != null)
                for (int i = 0; i < shaders.Length; i++)
                    if (shaders[i] != null)
                        shaders[i].Delete();
            
            Texture[] textures = Texture.Cache.ToArray();

            if (textures != null)
                for (int i = 0; i < textures.Length; i++)
                    if (textures[i] != null)
                        textures[i].Delete();

            base.OnUnload(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            WEngine.Stop(this);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            /*try
            {*/
                object obj = new object();

                ViewportDoOnceDelegate once = DoOnce;
                once?.Invoke();
                DoOnce -= once;

                Input.SetMouseScroll(Mouse.GetState().WheelPrecise);

                Time.DeltaTime = e.Time * Time.TimeScale;
                
                MouseState ms = Mouse.GetState();
                Vector2D delta = new Vector2D(this._PreviousState.X - ms.X, this._PreviousState.Y - ms.Y);

                this._PreviousState = ms;


                Update?.Invoke(new UpdateEventArgs(e.Time));

                GC.Collect();

                base.OnUpdateFrame(e);
            /*}
            catch(Exception err)
            {
                Debug.LogException(err);
            }*/
        }
    }
}
