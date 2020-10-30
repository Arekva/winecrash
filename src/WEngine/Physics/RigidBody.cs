using System.Diagnostics;

namespace WEngine
{
    public sealed class RigidBody : Module
    {
        private static object _VelocityLocker = new object();
        private Vector3D _Velocity = Vector3D.Zero;

        public Vector3D Velocity
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
        }
        public bool UseGravity { get; set; } = true;
        internal override void PreFixedUpdate()
        {
            if (UseGravity)
                this.Velocity += Physics.Gravity * Time.FixedDeltaTime;
        }

        protected internal override void Update()
        {
            this.WObject.Position += Velocity * Time.DeltaTime;
        }
    }
}
