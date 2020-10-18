using Newtonsoft.Json;
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
        
        [JsonConstructor]
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

        public Entity Parse()
        {
            try
            {
                Entity entity = Entity.Get(this.GUID);

                if (entity != null)
                {
                    //Debug.Log($"Updating Entity [{entity.GetType().Name}] {GUID}");

                    entity.WObject.Position = Position;
                }
                else
                {
                    WObject ettWobj = new WObject("Entity " + GUID)
                    {
                        Position = Position,
                    };
                    entity = (Entity)ettWobj.AddModule(this.EntityType);
                    entity.Identifier = GUID;
                    entity.Rotation = Rotation;

                    //Debug.Log($"Created Entity [{ent.GetType().Name}] {GUID}");
                }
                
                entity.Rotation = Rotation;

                return entity;
            }
            catch(Exception e)
            {
                Debug.LogError("Error when trying to parse entity: " + e.Message);
                return null;
            }
        }
    }
}