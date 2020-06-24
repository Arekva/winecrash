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

        private readonly float[] _Vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, -0.5f, 1.0f, 1.0f, // top right     - 0
             0.5f, -0.5f, -0.5f, 1.0f, 0.0f, // bottom right  - 1
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom left   - 2
            -0.5f,  0.5f, -0.5f, 0.0f, 1.0f  // top left      - 3
        };

        private readonly uint[] _Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _VertexBufferObject;
        private int _ElementBufferObject;
        private int _VertexArrayObject;

        private Shader _Shader;
        private Texture _Texture;
        private Texture _Texture2;

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
            try
            {
                GL.ClearColor(0.11f, 0.11f, 0.11f, 1.0f);

                this._VertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this._VertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, this._Vertices.Length * sizeof(float), this._Vertices, BufferUsageHint.StaticDraw);

                this._ElementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._ElementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, this._Indices.Length * sizeof(uint), this._Indices, BufferUsageHint.StaticDraw);

                this._Shader = new Shader("assets/shaders/shader.vert", "assets/shaders/shader.frag");
                this._Shader.Use();

                this._Texture = new Texture("assets/textures/container.png");
                this._Texture.Use(TextureUnit.Texture0);

                this._Texture2 = new Texture("assets/textures/awesomeface.png");
                this._Texture2.Use(TextureUnit.Texture1);

                this._Shader.SetInt("texture0", 0);
                this._Shader.SetInt("texture1", 1);

                this._VertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(this._VertexArrayObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this._VertexArrayObject);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._ElementBufferObject); //?

                int vertexLocation = _Shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                var texCoordLocation = _Shader.GetAttribLocation("aTexCoord");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                base.OnLoad(e);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindVertexArray(_VertexArrayObject);

            _Texture.Use();
            _Texture2.Use(TextureUnit.Texture1);
            _Shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, _Indices.Length, DrawElementsType.UnsignedInt, 0);

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

            _Shader.Delete();
            _Texture.Delete();

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
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Time.DeltaTime = e.Time;

            Update?.Invoke(new UpdateEventArgs(e.Time));

            GC.Collect();

            base.OnUpdateFrame(e);
        }

        

        
    }
}
