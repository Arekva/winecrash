using System.Runtime.CompilerServices;
using WEngine;

namespace Winecrash.Entities
{
    public class ItemEntity : Entity
    {

        protected override void Creation()
        {
            Collider.Extents = Vector3D.One * 0.1D;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            
        }

        protected override void OnDelete()
        {
            
        }
    }
}