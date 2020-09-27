using System;
using System.Collections.Generic;
using System.Linq;
using WEngine;

namespace Winecrash.Entities
{
    public abstract class Entity : Module
    {
        public static List<Entity> Entities { get; private set; }= new List<Entity>();
        public static object EntitiesLocker { get; }= new object();

        protected override void Creation()
        {
            lock(EntitiesLocker)
                Entities.Add(this);
        }

        protected override void OnDelete()
        {
            lock(EntitiesLocker)
                Entities.Remove(this);
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