using System.Diagnostics;
using System.Threading;

namespace WEngine
{
    public sealed class RigidBody : Module
    {
        public Vector3D Velocity { get; set; } = Vector3D.Zero;
        public double Drag { get; set; } = 0.00D;
        
        public bool UseGravity { get; set; } = true;
        
        protected internal override void EarlyPhysicsUpdate()
        {
            if (UseGravity) Velocity += Physics.Gravity * Time.PhysicsDelta;

            Velocity -= Velocity * Drag * Time.PhysicsDelta;
            
            WObject.Position += Velocity * Time.PhysicsDelta;
        }
    }
}
