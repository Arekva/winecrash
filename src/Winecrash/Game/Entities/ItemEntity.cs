using System.Runtime.CompilerServices;
using WEngine;

namespace Winecrash.Entities
{
    public class ItemEntity : Entity
    {
        public ContainerItem Item { get; set; }
        
        protected override void Creation()
        {
            Collider.Extents = Vector3D.One * 0.1D;
        }

        protected override void Start()
        {
            base.Start();
            
            
        }

        protected override void OnDelete()
        {
            Item = null;
            //Item?.Delete();
        }
    }
}