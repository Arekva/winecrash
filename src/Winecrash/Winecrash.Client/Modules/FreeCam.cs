using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Winecrash.Game
{
    public class FreeCam : Module
    {
        public double MoveSpeed = 20.0F;
        public double MoveSpeedSensivity = 5.0F;
        public double MoveScrollSensivity = 200.0F;
        private Vector2D Angles = new Vector2D();

        public Camera EditedCamera { get; set; }


        public static bool FreeCTRL = false;

        protected override void Creation()
        {
            EditedCamera = Camera.Main;

            //Input.MouseSensivity *= 5.0F;
        }

        protected override void Update()
        {
            if(Input.IsPressing(Keys.End))
            {
                FreeCTRL = !FreeCTRL;
            }

            if (!FreeCTRL) return;

            double newSpeed = Input.MouseScrollDelta * MoveScrollSensivity * Time.DeltaTime;

            this.MoveSpeed += newSpeed + MoveSpeed * Input.MouseScrollDelta * 0.2D;

            this.MoveSpeed = WMath.Clamp(this.MoveSpeed, 0.01D, double.PositiveInfinity);
            //if(Input.IsPressed(Keys.Mouse))
            //Debug.Log(this.WObject.Position);

            /*Vector2D deltas = Input.MouseDelta;

            double ax = (Angles.X + (deltas.X * Input.MouseSensivity * (float)Time.DeltaTime)) % 360.0F;
            double ay = WMath.Clamp((Angles.Y + (deltas.Y * Input.MouseSensivity * (float)Time.DeltaTime)), -90.0F, 90.0F);

            Angles = new Vector2D(ax, ay);

            this.WObject.Rotation = new Quaternion(-ay, ax, 0.0F);*/

            Vector3D fwd = this.WObject.Forward;
            Vector3D rght = this.WObject.Right;
            Vector3D up = this.WObject.Up;

            Vector3D pos = this.WObject.Position;

            if (Input.IsPressed(Keys.Z))
            {
                fwd *= MoveSpeed * Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.S))
            {
                fwd *= -MoveSpeed * Time.DeltaTime;
            }
            else
            {
                fwd *= 0.0D;
            }

            if (Input.IsPressed(Keys.Q))
            {
                rght *= -MoveSpeed * Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.D))
            {
                rght *= MoveSpeed * Time.DeltaTime;
            }
            else
            {
                rght *= 0.0D;
            }

            if (Input.IsPressed(Keys.Space))
            {
                up *= MoveSpeed * Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.LeftShift))
            {
                up *= -MoveSpeed * Time.DeltaTime;
            }
            else
            {
                up *= 0.0D;
            }

            Player.Instance.WObject.Position += fwd - rght + up;
             
            
            if(Input.IsPressed(Keys.F3) && Input.IsPressing(Keys.A))
            {
                Debug.Log("Reconstructing");
                Debug.Log(Chunk.Chunks.Count);
                foreach (Chunk chunk in Chunk.Chunks)
                {
                    Task.Run(chunk.Construct);
                }
            }

            if(Input.IsPressing(Keys.K))
            {
                //Player.Instance.WObject.Position = new Vector3F(140_000_000F, 128F, 0.0F);
                Player.Instance.ForceInvokeChunkChange();
            }
        }
    }
}
