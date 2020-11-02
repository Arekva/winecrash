using System.Collections.Generic;
using WEngine.Debugging;

namespace WEngine
{
    public class BoxCollider : AABBCollider
    {
        internal static List<BoxCollider> BoxColliders = new List<BoxCollider>();
        internal static object BoxCollidersLocker { get; private set; } = new object();
        internal static List<BoxCollider> ActiveBoxColliders = new List<BoxCollider>();
        internal static object ActiveBoxCollidersLocker { get; private set; } = new object();

        private Vector3D _Extents = Vector3D.One / 2.0D;
        public override Vector3D Extents
        {
            get
            {
                return _Extents;
            }

            set
            {
                _Extents = value;
#if DEBUG
                if(DebugVolumeWobject) DebugVolumeWobject.LocalScale = value * 2;
#endif
            }
        }
        
        
        private Vector3D _Offset = Vector3D.Zero;
        public Vector3D Offset
        {
            get
            {
                return _Offset;
            }

            set
            {
                _Offset = value;
#if DEBUG
                if(DebugVolumeWobject) DebugVolumeWobject.LocalPosition = value;
#endif
            }
        }

#if DEBUG
        private WObject DebugVolumeWobject;
#endif

        public override Vector3D Center
        {
            get
            {
                return this.WObject.Position + Offset;
            }
            set
            {
                this.WObject.Position = value - Offset;
            }
        }

        public override AABB AABB
        {
            get
            {
                return new AABB(this.Center, this.Extents);
            }

            set
            {
                this.Extents = value.Extents;
                this.Center = value.Position;
            }
        }

        protected internal override void Creation()
        {
            lock(BoxCollidersLocker)
            { BoxColliders.Add(this); }
            
            
#if DEBUG
            /*if (Engine.DoGUI)
            {

                DebugVolumeWobject = new WObject("Debug Volume") {Enabled = true};
                DebugVolumeWobject.Parent = this.WObject;


                BoxDebugVolume debugVolume = DebugVolumeWobject.AddModule<BoxDebugVolume>();
                debugVolume.Extents = this.Extents;
                debugVolume.Offset = this.Offset;
            }*/

#endif
            
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
            
#if DEBUG
            DebugVolumeWobject?.Delete();
            DebugVolumeWobject = null;
#endif
            base.OnDelete();
        }

        
    }
}
