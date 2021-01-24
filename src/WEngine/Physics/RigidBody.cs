using System.Diagnostics;
using System.Threading;

namespace WEngine
{
    public sealed class RigidBody : Module
    {
        //private object _VelocityLocker = new object();
        public Vector3D Velocity { get; set; } = Vector3D.Zero;

        /*public Vector3D Velocity
        {
            get
            {
                lock (_VelocityLocker)
                {
                    return _Velocity;
                }
            }

            set
            {
                lock (_VelocityLocker)
                {
                    _Velocity = value;
                }
            }
        }*/
        public bool UseGravity { get; set; } = true;
        
        protected internal override void EarlyUpdate()
        {
            if (UseGravity)
                this.Velocity += Physics.Gravity * Time.PhysicsDelta;
            
            this.WObject.Position += Velocity * Time.PhysicsDelta;
        }

        /*protected internal override void LateFixedUpdate()
        {
            
        }*/
    }
}
