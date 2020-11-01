using System.Collections.Generic;
using WEngine.Debugging;

namespace WEngine
{
    public abstract class AABBCollider : Module, ICollider
    {
        public virtual Vector3D Extents { get; set; }
        
        public virtual Vector3D Center { get; set; }
        public virtual AABB AABB { get; set; }
    }
}
