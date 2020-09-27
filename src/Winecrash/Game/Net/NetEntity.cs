using System;
using WEngine;
using WEngine.Networking;
using Winecrash.Entities;

namespace Winecrash.Net
{
    public class NetEntity : NetObject
    {
        public Guid GUID { get; set; }
        public Type EntityType { get; set; }
        public Vector3D Position { get; set; }
        public Quaternion Rotation { get; set; }
        

        public NetEntity(Guid id, Type type, Vector3D pos, Quaternion rot)
        {
            this.GUID = id;
            this.EntityType = type;
            this.Position = pos;
            this.Rotation = rot;
        }
        
        public NetEntity(Entity entity)
        {
            this.GUID = entity.Identifier;
            this.EntityType = entity.GetType();
            this.Position = entity.WObject.Position;
            this.Rotation = entity.WObject.Rotation;
        }

        public void Parse()
        {
            Entity entity = Entity.Get(this.GUID);

            if (entity != null)
            {
                entity.WObject.Position = Position;
                entity.WObject.Rotation = Rotation;
            }
            else
            {
                
            }
        }
    }
}