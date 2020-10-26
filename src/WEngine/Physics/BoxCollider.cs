using System.Collections.Generic;

namespace WEngine
{
    public sealed class BoxCollider : Module, ICollider
    {
        internal static List<BoxCollider> BoxColliders = new List<BoxCollider>();
        internal static object BoxCollidersLocker { get; private set; } = new object();
        internal static List<BoxCollider> ActiveBoxColliders = new List<BoxCollider>();
        internal static object ActiveBoxCollidersLocker { get; private set; } = new object();

        public Vector3D Extents { get; set; } = Vector3D.One / 2.0D;
        
        public Vector3D Offset { get; set; } = Vector3D.Zero;

        public Vector3D Center
        {
            get
            {
                return this.WObject.Position + Offset;
            }
        }

        protected internal override void Creation()
        {
            lock(BoxCollidersLocker)
            { BoxColliders.Add(this); }
            base.Creation();
        }

        protected internal override void OnEnable()
        {
            lock(ActiveBoxCollidersLocker)
            { ActiveBoxColliders.Add(this); }
            base.OnEnable();
        }


        protected internal override void OnDisable()
        {
            lock(ActiveBoxCollidersLocker)
            { ActiveBoxColliders.Remove(this); }
            base.OnEnable();
        }

        protected internal override void OnDelete()
        {
            lock(ActiveBoxCollidersLocker)
            { ActiveBoxColliders.Remove(this); }
            lock(BoxCollidersLocker)
            { BoxColliders.Remove(this); }
            base.OnDelete();
        }
    }
}
