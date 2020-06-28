using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public abstract class Collider : Module
    {
        internal List<Collider> Colliders = new List<Collider>();
        internal List<Collider> ActiveColliders = new List<Collider>();

        protected internal override void Creation()
        {
            Colliders.Add(this);
        }

        protected internal override void OnDisable()
        {
            ActiveColliders.Remove(this);
        }

        protected internal override void OnEnable()
        {
            ActiveColliders.Add(this);
        }

        protected internal override void OnDelete()
        {
            ActiveColliders.Remove(this);
            Colliders.Remove(this);
        }
    }
}
