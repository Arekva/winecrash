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

namespace Winecrash.Engine
{
    public class Viewport : GameWindow
    {
        public static Viewport Instance { get; private set; }

        public static Thread ThreadRunner;

        internal static Updater.UpdateCallback OnFrameStart;

        public static event UpdateEventHandler Update;

        public Viewport(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) 
        {
            Instance = this;
        }

        protected override void OnLoad(EventArgs e)
        {
            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            GL.ClearColor(0.11f, 0.11f, 0.11f, 1.0f);

            //création d'un buffer pour les vertex
            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();

            

            GL.BindVertexArray(VertexArrayObject);

            //assignation du buffer en temps que vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //copie du tableau de vertices dans le buffer
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            //En gros:
            // 1er arg     : 0 => numéro de l'attribut voulu: ici numéro 0 (location)
            // 2eme arg    : 3 => spécifie la taille de l'attribut du vertex, ici 3 octets (vector3)
            // 3eme arg    : spécifie le type de donnée, ici float.
            // 4eme arg    : normalisation: en gros, envoie -1 si nombre négatif, et 1 si positif
            // 5eme arg    : position de la prochaine donnée en octet (ici la taille du vecteur)
            // 6eme arg    : offset du début de la donnée par rapport à la position
            GL.VertexAttribPointer(shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
            shader.Dispose();
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
            Update?.Invoke(new UpdateEventArgs(e.Time));

            //Layer.Update();

            //Debug.Log((int)(1D/e.Time));

            GC.Collect();

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        float[] vertices = {
            -0.5f, -0.5f, 0.0f,  //Bottom-left vertex
             0.5f, -0.5f, 0.0f,  //Bottom-right vertex
            -0.5f,  0.5f, 0.0f,  //Top-left vertex

             0.0f,  0.0f, 0.0f,  //Bottom-left vertex
             1.0f,  0.0f, 0.0f,  //Bottom-right vertex
             0.0f,  1.0f, 0.0f,  //Top-left vertex
        };

        int VertexBufferObject;
        int VertexArrayObject;

        Shader shader;
    }
}
