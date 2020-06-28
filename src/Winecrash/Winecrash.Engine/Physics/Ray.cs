using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public struct Ray
    {
        public Vector3D Origin { get; }
        public Vector3D Direction { get; }

        public Ray(Vector3D origin, Vector3D dir)
        {
            this.Origin = origin;
            this.Direction = dir.Normalized;
        }
    }
}
