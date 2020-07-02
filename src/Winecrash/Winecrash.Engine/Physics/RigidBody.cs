using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public sealed class RigidBody : Module
    {
        public Vector3D Velocity { get; set; } = Vector3D.Zero;
        public bool UseGravity { get; set; } = true;

        protected internal override void Update()
        {
            if (UseGravity)
                this.Velocity += Physics.Gravity * Time.DeltaTime;
        }

        protected internal override void LateUpdate()
        {
            this.WObject.Position += (Vector3F)Velocity;
        }
    }
}
