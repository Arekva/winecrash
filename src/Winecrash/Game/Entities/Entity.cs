using System;
using System.Collections.Generic;
using System.Linq;
using WEngine;

namespace Winecrash.Entities
{
    public delegate void EntityRotationUpdate(Quaternion quaternion);
    
    public abstract class Entity : Module
    {
        public static List<Entity> Entities { get; private set; } = new List<Entity>();
        public static object EntitiesLocker { get; } = new object();

        public event EntityRotationUpdate OnRotate;

        private Quaternion _Rotation;
        public Quaternion Rotation
        {
            get
            {
                return this._Rotation;
            }
            set
            {
                _Rotation = value;
                OnRotate?.Invoke(value);
            }
        }

        protected override void Creation()
        {
            lock(EntitiesLocker)
                Entities.Add(this);
        }

        protected override void OnDelete()
        {
            lock(EntitiesLocker)
                Entities.Remove(this);

            OnRotate = null;
        }

        public static Entity Get(Guid guid)
        {
            Entity[] entities = null;
            lock (EntitiesLocker)
                entities = Entities.ToArray();

            return entities.FirstOrDefault(e => e.Identifier == guid);
        }
    }
}