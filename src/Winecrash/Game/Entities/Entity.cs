using System;
using System.Collections.Generic;
using System.Linq;
using WEngine;

namespace Winecrash.Entities
{
    public delegate void EntityChunkChange(Vector2I previousChunk, Vector2I newChunk);

    public delegate void EntityDimensionChange(string previousDimension, string newDimension);
        
    public delegate void EntityRotationUpdate(Quaternion quaternion);
    
    public abstract class Entity : Module, ICollider
    {
        public static List<Entity> Entities { get; private set; } = new List<Entity>();
        public static object EntitiesLocker { get; } = new object();
        
        public RigidBody RigidBody { get; private set; } = null;
        public BoxCollider Collider { get; private set; } = null;
        
        
        public event EntityRotationUpdate OnRotate;
        public event EntityChunkChange OnChunkChange;
        public event EntityDimensionChange OnDimensionChange;

        private Chunk _chunk = null;

        public Chunk Chunk
        {
            get => _chunk;
            set
            {
                // switch this from other collections
                if (_chunk)
                    lock (_chunk.EntityLocker)
                        _chunk.Entities.Remove(this);

                if(value)
                    lock (value.EntityLocker)
                        value.Entities.Add(this);
                    
                this._chunk = value;
            }
        }
        

        public Vector2I ChunkCoordinates
        {
            get
            { 
                World.GlobalToLocal(this.WObject.Position, out Vector2I cpos, out _);
                return cpos;
            }
        }
        public Vector3I BlockCoordinates
        {
            get
            {
                World.GlobalToLocal(this.WObject.Position, out _, out Vector3I bpos);
                return bpos;
            }
        }
        public Vector3I Coordinates
        {
            get
            {
                return (Vector3I)this.WObject.Position;
            }
        }

        private Vector2I previousChunk;

        public string Dimension { get; set; } = "winecrash:overworld";
        
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

            this.RigidBody = this.WObject.AddModule<RigidBody>();
            this.Collider = this.WObject.AddModule<BoxCollider>();
            
            OnChunkChange += ChunkChange;
            
            //this.WObject
            RigidBody.UseGravity = false;
        }

        protected override void FirstFrame()
        {
            base.FirstFrame();

            this.Chunk = World.GetChunk(this.ChunkCoordinates, this.Dimension);
        }

        protected override void Update()
        {
            base.Update();

            Vector2I currentChunk = this.ChunkCoordinates;
            
            if (currentChunk != previousChunk)
            {
                OnChunkChange?.Invoke(previousChunk, currentChunk);
            }

            previousChunk = currentChunk;
        }

        protected override void OnDelete()
        {
            lock(EntitiesLocker)
                Entities.Remove(this);
            
            this.Chunk = null;

            OnRotate = null;
            //OnChunkChange -= ChunkChange;
            OnChunkChange = null;
            OnDimensionChange = null;

            RigidBody?.Delete();
            RigidBody = null;

            Collider?.Delete();
            Collider = null;
        }


        private void ChunkChange(Vector2I previousChunk, Vector2I newChunk)
        {
            /*Chunk c = this.Chunk;

            if (c != null)
            {
                lock (c.EntityLocker)
                {
                    c.Entities.Remove(this);
                }
            }

            c = */this.Chunk = World.GetChunk(newChunk, this.Dimension);
            
            // todo: if chunk is null, save entity into chunk file and delete.
            // this will for sure throw a nullref if an entity tried to go
            // into an unloaded chunk.

            /*if (c != null)
            {
                lock (c.Entities)
                    c.Entities.Add(this);
                this.WObject.Parent = c.WObject;
            }*/
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