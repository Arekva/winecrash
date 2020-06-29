using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Winecrash.Client
{
    public class FreeCam : Module
    {
        public float MoveSpeed = 5.0F;
        private Vector2D Angles = new Vector2D();

        public Camera EditedCamera { get; set; }

        protected override void Creation()
        {
            EditedCamera = Camera.Main;

            Input.MouseSensivity *= 5.0F;
        }

        protected override void Update()
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


            if(Input.IsPressed(Keys.Delete))
            {
                World.Instance.WObject.Delete();

                WEngine.TraceLayers();
            }
        }
    }
}
