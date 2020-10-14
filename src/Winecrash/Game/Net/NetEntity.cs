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

        public void Parse()
        {
            try
            {
                Entity entity = Entity.Get(this.GUID);

                if (entity != null)
                {
                    //Debug.Log("Updating Entity " + GUID);
                    entity.WObject.Position = Position;
                    entity.WObject.Rotation = Rotation;
                }
                else
                {
                    WObject ettWobj = new WObject("Entity " + GUID);
                    Entity ent = (Entity)ettWobj.AddModule(this.EntityType);
                    ent.Identifier = GUID;

                    Debug.Log($"Created Entity [{ent.GetType().Name}] {GUID}");
                }
            }
            catch(Exception e)
            {
                Debug.LogError("Error when trying to parse entity: " + e.Message);
            }
        }
    }
}