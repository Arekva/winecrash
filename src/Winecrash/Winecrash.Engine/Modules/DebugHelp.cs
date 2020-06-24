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

        public static Vector3F ModelShift = Vector3F.Zero;
        public static Quaternion Rotation = Quaternion.Identity;
        public static float Scale = 1.0F;

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
                ModelShift.Z += ShiftSensivity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.S))
            {
                ModelShift.Z -= ShiftSensivity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.Q))
            {
                ModelShift.X -= ShiftSensivity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.D))
            {
                ModelShift.X += ShiftSensivity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.A))
            {
                ModelShift.Y += ShiftSensivity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.E))
            {
                ModelShift.Y -= ShiftSensivity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.E))
            {
                ModelShift.Y -= ShiftSensivity * (float)Time.DeltaTime;
            }
            #endregion

            #region rotation
            if (Input.IsPressed(Keys.NumPad5))
            {
                Rotation = Quaternion.Identity;
            }

            if (Input.IsPressed(Keys.NumPad8))
            {
                Rotation *= new Quaternion(Vector3D.Right, RotationSensivity * (float)Time.DeltaTime);
                //EulerAngles.X += RotationSensivity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.NumPad2))
            {
                Rotation *= new Quaternion(Vector3D.Right, -RotationSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.NumPad6))
            {
                Rotation *= new Quaternion(Vector3D.Up, RotationSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.NumPad4))
            {
                Rotation *= new Quaternion(Vector3D.Up, -RotationSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.NumPad9))
            {
                Rotation *= new Quaternion(Vector3D.Forward, RotationSensivity * (float)Time.DeltaTime);
            }

            if (Input.IsPressed(Keys.NumPad7))
            {
                Rotation *= new Quaternion(Vector3D.Forward, -RotationSensivity * (float)Time.DeltaTime);
            }
            #endregion

            #region scale
            if (Input.IsPressed(Keys.PageUp))
            {
                Scale += ScaleSenvity * (float)Time.DeltaTime;
            }

            if (Input.IsPressed(Keys.PageDown))
            {
                Scale -= ScaleSenvity * (float)Time.DeltaTime;
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
