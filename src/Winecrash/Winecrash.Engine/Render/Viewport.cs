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
        public static event ViewportLoadDelegate OnLoaded;

        public delegate void ViewportLoadDelegate();

        MouseState _PreviousState = new MouseState();

        private readonly float[] _Vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, -0.5f, 1.0f, 1.0f, // top right     - 0
             0.5f, -0.5f, -0.5f, 1.0f, 0.0f, // bottom right  - 1
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom left   - 2
            -0.5f,  0.5f, -0.5f, 0.0f, 1.0f  // top left      - 3
        };

        internal float[] cube_vertices = {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};

        internal readonly uint[] _Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        internal int _VertexBufferObject;
        internal int _ElementBufferObject;
        internal int _VertexArrayObject;

        internal Shader _Shader;
        internal Material Mat;
        internal Texture _Texture;
        internal Texture _Texture2;

        internal Matrix4 _View;
        internal Matrix4 _Projection;

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
            WObject camWobj = new WObject("Main Camera");
            camWobj.AddModule<Camera>();

            Shader.CreateError();

            try
            {
                
                this._VertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this._VertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, this./*_Vertices*/cube_vertices.Length * sizeof(float), this./*_Vertices*/cube_vertices, BufferUsageHint.StaticDraw);

                this._ElementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._ElementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, this._Indices.Length * sizeof(uint), this._Indices, BufferUsageHint.StaticDraw);

                this._Shader = new Shader("assets/shaders/Standard.vert", "assets/shaders/Standard.frag");

                Mat = WObject.Find("Engine Core").AddOrGetModule<MeshRenderer>().Material = new Material(this._Shader);

                Mat.SetData<Texture>("albedo", new Texture("assets/textures/container.png"));
                Mat.SetData<Texture>("texture1", new Texture("assets/textures/awesomeface.png"));

                this._VertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(this._VertexArrayObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this._VertexArrayObject);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._ElementBufferObject);

                int vertexLocation = _Shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                var texCoordLocation = _Shader.GetAttribLocation("aTexCoord");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));


                
                
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Error when loading Viewport: ", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                base.OnLoad(e);
                OnLoaded?.Invoke();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Render?.Invoke(new UpdateEventArgs(e.Time));

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_VertexBufferObject);
            GL.DeleteVertexArray(_VertexArrayObject);


            
            Shader[] shaders = Shader.Cache.ToArray();
            if(shaders != null)
                for (int i = 0; i < shaders.Length; i++)
                    if (shaders[i] != null)
                        shaders[i].Delete();
            shaders = null;

            
            Texture[] textures = Texture.Cache.ToArray();
            System.Windows.Forms.MessageBox.Show(textures.Length.ToString());
            if (textures != null)
                for (int i = 0; i < textures.Length; i++)
                    if (textures[i] != null)
                        textures[i].Delete();
            textures = null;

            //Thread.Sleep(5000);

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
            Time.DeltaTime = e.Time;
            MouseState ms = Mouse.GetState();
            Vector2D delta = new Vector2D(this._PreviousState.X - ms.X, this._PreviousState.Y - ms.Y);
            Input.MouseDelta = delta;
            this._PreviousState = ms;
            if (Input.LockMode == CursorLockModes.Lock && Focused)
            {
                
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            Update?.Invoke(new UpdateEventArgs(e.Time));

            GC.Collect();

            base.OnUpdateFrame(e);
        }
    }
}
