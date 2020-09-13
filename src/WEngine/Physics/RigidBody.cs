namespace WEngine
{
    public sealed class RigidBody : Module
    {
        public Vector3D Velocity { get; set; } = Vector3D.Zero;
        public bool UseGravity { get; set; } = true;

        protected internal override void FixedUpdate()
        {
            if (UseGravity)
                this.Velocity += Physics.Gravity * Time.FixedDeltaTime;
        }

        internal override void LateFixedUpdate()
        {
            this.WObject.Position += Velocity * Time.FixedDeltaTime;
        }
    }
}
