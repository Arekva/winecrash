using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;

namespace Winecrash.Engine
{
    public sealed class Camera : Module
    {
        internal static List<Camera> Cameras { get; set; } = new List<Camera>(1);

        public static Camera Main { get; set; }

#if DEBUG
        public float MoveSpeed = 5.0F;
        

        private Vector2D Angles = new Vector2D();

        protected internal override void Creation()
        {
            Input.MouseSensivity *= 5.0F;

            Cameras.Add(this);

            if (Camera.Main == null)
                Camera.Main = this;
        }

        protected internal override void Update()
        {
            
           
            Vector2D deltas = Input.MouseDelta;

            double ax = (Angles.X + (deltas.X * Input.MouseSensivity * (float)Time.DeltaTime)) % 360.0F;
            double ay = WMath.Clamp((Angles.Y + (deltas.Y * Input.MouseSensivity * (float)Time.DeltaTime)), -90.0F, 90.0F);

            Angles = new Vector2D(ax, ay);

            this.WObject.Rotation = new Quaternion(-ay, ax, 0.0F);
            
            Vector3F fwd = this.WObject.Forward;
            Vector3F rght = this.WObject.Right;
            Vector3F up = this.WObject.Up;

            Vector3F pos = this.WObject.Position;

            if (Input.IsPressed(Keys.Z))
            {
                fwd *= MoveSpeed * (float)Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.S))
            {
                fwd *= -MoveSpeed * (float)Time.DeltaTime;
            }
            else
            {
                fwd *= 0.0F;
            }

           if (Input.IsPressed(Keys.Q))
            {
                rght *= -MoveSpeed * (float)Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.D))
            {
                rght *= MoveSpeed * (float)Time.DeltaTime;
            }
            else
            {
                rght *= 0.0F;
            }

            if (Input.IsPressed(Keys.Space))
            {
                up *= MoveSpeed * (float)Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.LeftShift))
            {
                up *= -MoveSpeed * (float)Time.DeltaTime;
            }
            else
            {
                up *= 0.0F;
            }

            this.WObject.Position = pos + fwd - rght + up;
        }

#else
        protected internal override void Creation()
        {
            Cameras.Add(this);

            if (Camera.Main == null)
                Camera.Main = this;
        }
#endif
        protected internal override void OnDelete()
        {
            Cameras.Remove(this);

            if(Main != null && Main == this)
            {
                Main = null;
            }
        }

        protected internal override void OnRender()
        {
            Vector3F p = this.WObject.Position;
            Vector3F t = this.WObject.Forward;
            Vector3F u = this.WObject.Up;

            Viewport vp = Viewport.Instance;

            vp._View = Matrix4.LookAt(new Vector3(p.X, p.Y, p.Z), new Vector3(p.X + t.X, p.Y + t.Y, p.Z + t.Z), new Vector3(u.X, u.Y, u.Z));

            //Debug.Log(p);

            GL.BindVertexArray(vp._VertexArrayObject);

            vp._Texture.Use();
            vp._Texture2.Use(TextureUnit.Texture1);
            vp._Shader.Use();

            if (EngineCore.Instance != null)
            {
                //vp._View = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
                vp._Projection = Matrix4.CreatePerspectiveFieldOfView(45.0F * (float)WMath.DegToRad, (float)vp.Width / (float)vp.Height, 0.1f, 100.0f);
                WObject wobj = EngineCore.Instance.WObject;

                //Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)this.Width / (float)this.Height, 0.1f, 100.0f);

                Matrix4 mat = wobj.TransformMatrix * vp._View * vp._Projection;//wobj.TransformMatrix;

                vp._Shader.SetMatrix4("transform", mat);

                //vp._Shader.SetMatrix4("model", wobj.TransformMatrix);
                //vp._Shader.SetMatrix4("view", vp._View);
                //vp._Shader.SetMatrix4("projection", vp._Projection);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                //GL.DrawElements(PrimitiveType.Triangles, _Indices.Length, DrawElementsType.UnsignedInt, 0);

                /*foreach (WObject child in wobj.Children)
                {
                    _Shader.SetMatrix4("transform", child.TransformMatrix);
                    GL.DrawElements(PrimitiveType.Triangles, _Indices.Length, DrawElementsType.UnsignedInt, 0);
                }*/

            }
        }
    }
}
