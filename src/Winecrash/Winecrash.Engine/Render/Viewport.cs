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
    class test : Module
    {
        protected internal override void Update()
        {
            Vector3D a = this.WObject.LocalRotation.Euler;
            this.WObject.LocalRotation = new Quaternion(a.X, a.Y + 10 * (float)Time.DeltaTime, a.Z);
        }
    }
    public class Viewport : GameWindow
    {
        public static Viewport Instance { get; private set; }

        internal static Thread ThreadRunner { get; set; }

        public static event UpdateEventHandler Update;
        public static event UpdateEventHandler Render;
        public static event ViewportLoadDelegate OnLoaded;

        public delegate void ViewportLoadDelegate();

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
            WObject camWobj = new WObject("Main Camera");
            camWobj.AddModule<Camera>();

            Shader.CreateError();

            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            

            

            /*glDepthMask(GL_TRUE);
            glDepthFunc(GL_LEQUAL);
            glDepthRange(0.0f, 1.0f);*/

            try
            {
                
                /*this._VertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this._VertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, this.cube_vertices.Length * sizeof(float), this.cube_vertices, BufferUsageHint.StaticDraw);

                this._ElementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._ElementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, this._Indices.Length * sizeof(uint), this._Indices, BufferUsageHint.StaticDraw);*/

                new Shader("assets/shaders/Standard.vert", "assets/shaders/Standard.frag");
                new Texture("assets/textures/container.png");

                WObject pedestal = new WObject("Pedestal");
                //pedestal.Rotation = new Quaternion(0, 90, 0);

                WObject wobj = new WObject("Cube");
                wobj.Parent = pedestal;
                wobj.LocalPosition = Vector3F.Zero;
                
                MeshRenderer mr = wobj.AddModule<MeshRenderer>();
                mr.Mesh = Mesh.LoadFile("assets/models/Cube.obj", MeshFormats.Wavefront);
                Material mat = mr.Material = new Material(Shader.Find("Standard"));
                mat.SetData<Texture>("albedo", Texture.Find("container"));
                mat.SetData<Vector4>("color", new Vector4(1,0,0,1));

                //wobj.AddModule<test>();

                

                WObject wobj1 = new WObject("Suzanne");
                //wobj1.Parent = pedestal;
                wobj1.LocalPosition = Vector3F.Left * 3.0F;
                MeshRenderer mr1 = wobj1.AddModule<MeshRenderer>();
                mr1.Mesh = Mesh.LoadFile("assets/models/Suzanne.obj", MeshFormats.Wavefront);
                Material mat1 = mr1.Material = new Material(Shader.Find("Standard"));
                //mat1.SetData<Texture>("albedo", Texture.Find("container"));
                mat1.SetData<Vector4>("color", new Vector4(0, 1, 0, 1));

                //wobj1.AddModule<test>();

                Debug.Log(wobj1.Position);

                WObject wobj2 = new WObject("Ico");
                //wobj2.Parent = pedestal;
                wobj2.LocalPosition = Vector3F.Right * 3.0F;
                MeshRenderer mr2 = wobj2.AddModule<MeshRenderer>();
                mr2.Mesh = Mesh.LoadFile("assets/models/Ico_1.obj", MeshFormats.Wavefront);
                Material mat2 = mr2.Material = new Material(Shader.Find("Standard"));
                //mat1.SetData<Texture>("albedo", Texture.Find("container"));
                mat2.SetData<Vector4>("color", new Vector4(0, 0, 1, 1));

                //wobj2.AddModule<test>();


                WObject mainLight = new WObject("Directional Light");
                mainLight.AddModule<DirectionalLight>();
                mainLight.LocalRotation = new Quaternion(50.0D, -30.0D - 90.0D, 0.0D);


                
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Layer.Render

            Render?.Invoke(new UpdateEventArgs(e.Time));
            
            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

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

        static Stopwatch updatesw = new Stopwatch();
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            
            //Debug.Log((1D/e.Time).ToString("C0").Split('¤')[1] + " FPS");
            Time.DeltaTime = e.Time;
            MouseState ms = Mouse.GetState();
            Vector2D delta = new Vector2D(this._PreviousState.X - ms.X, this._PreviousState.Y - ms.Y);
            Input.MouseDelta = Focused ? delta : Vector2D.Zero;
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
