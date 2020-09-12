using System.Collections.Generic;

namespace WEngine
{
    public sealed class BoxCollider : Collider
    {
        internal static List<BoxCollider> BoxColliders = new List<BoxCollider>();
        internal static List<BoxCollider> ActiveBoxColliders = new List<BoxCollider>();

        public Vector3D Extents { get; set; } = Vector3D.One;

        protected internal override void Creation()
        {
            BoxColliders.Add(this);
            base.Creation();
        }

        protected internal override void OnEnable()
        {
            ActiveBoxColliders.Add(this);
            base.OnEnable();
        }


        protected internal override void OnDisable()
        {
            ActiveBoxColliders.Remove(this);
            base.OnEnable();
        }

        protected internal override void OnDelete()
        {
            ActiveBoxColliders.Remove(this);
            BoxColliders.Remove(this);
            base.OnDelete();
        }
    }
}
