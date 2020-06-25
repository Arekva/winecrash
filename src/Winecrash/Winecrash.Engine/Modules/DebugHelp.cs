using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public class DebugHelp : Module
    {
        public static DebugHelp Instance { get; private set; }

        public static float ShiftSensivity = 5F;
        public static float RotationSensivity = 90.0F;
        public static float ScaleSenvity = 1F;

        protected internal override void Creation()
        {
            if(Instance)
            {
                this.Delete();
                return;
            }

            Instance = this;
        }

        protected internal override void Update()
        {
            #region shift
            if (Input.IsPressed(Keys.Z))
            {
                this.WObject.LocalPosition = new Vector3F(this.WObject.LocalPosition.X, this.WObject.LocalPosition.Y, this.WObject.LocalPosition.Z + ShiftSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.S))
            {
                this.WObject.LocalPosition = new Vector3F(this.WObject.LocalPosition.X, this.WObject.LocalPosition.Y, this.WObject.LocalPosition.Z - ShiftSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.Q))
            {
                this.WObject.LocalPosition = new Vector3F(this.WObject.LocalPosition.X - ShiftSensivity * (float)Time.DeltaTime, this.WObject.LocalPosition.Y, this.WObject.LocalPosition.Z);
            }

            if (Input.IsPressed(Keys.D))
            {
                this.WObject.LocalPosition = new Vector3F(this.WObject.LocalPosition.X + ShiftSensivity * (float)Time.DeltaTime, this.WObject.LocalPosition.Y , this.WObject.LocalPosition.Z);
            }

            if (Input.IsPressed(Keys.A))
            {
                this.WObject.LocalPosition = new Vector3F(this.WObject.LocalPosition.X, this.WObject.LocalPosition.Y + ShiftSensivity * (float)Time.DeltaTime, this.WObject.LocalPosition.Z);
            }

            if (Input.IsPressed(Keys.E))
            {
                this.WObject.LocalPosition = new Vector3F(this.WObject.LocalPosition.X, this.WObject.LocalPosition.Y - ShiftSensivity * (float)Time.DeltaTime, this.WObject.LocalPosition.Z);
            }

            #endregion

            #region rotation
            if (Input.IsPressed(Keys.NumPad5))
            {
                this.WObject.LocalRotation = Quaternion.Identity;
            }

            if (Input.IsPressed(Keys.NumPad8))
            {
                this.WObject.LocalRotation *= new Quaternion(Vector3D.Right, RotationSensivity * (float)Time.DeltaTime);
                //EulerAngles.X += RotationSensivity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.NumPad2))
            {
                this.WObject.LocalRotation *= new Quaternion(Vector3D.Right, -RotationSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.NumPad6))
            {
                this.WObject.LocalRotation *= new Quaternion(Vector3D.Up, RotationSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.NumPad4))
            {
                this.WObject.LocalRotation *= new Quaternion(Vector3D.Up, -RotationSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.NumPad9))
            {
                this.WObject.LocalRotation *= new Quaternion(Vector3D.Forward, RotationSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.NumPad7))
            {
                this.WObject.LocalRotation *= new Quaternion(Vector3D.Forward, -RotationSensivity * (float)Time.DeltaTime);
            }
            #endregion

            #region scale
            if (Input.IsPressed(Keys.PageUp))
            {
                this.WObject.Scale += new Vector3F(ScaleSenvity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.PageDown))
            {
                this.WObject.Scale -= new Vector3F(ScaleSenvity * (float)Time.DeltaTime);
            }
            #endregion
        }

        protected internal override void OnDelete()
        {
            if(Instance && Instance == this)
            {
                Instance = null;
            }
        }
    }
}
