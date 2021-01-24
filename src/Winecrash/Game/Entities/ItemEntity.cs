using System;
using WEngine;

namespace Winecrash.Entities
{
    public class ItemEntity : Entity
    {
        public ContainerItem Item { get; set; }
        
        public MeshRenderer Renderer { get; set; }
        
        protected override void Creation()
        {
            base.Creation();

            Vector3D scale = Vector3D.One * 0.35D;
            WObject.LocalScale = scale; 
            Collider.Extents = scale;

            Renderer = this.WObject.AddModule<MeshRenderer>();
            Material mat = ItemCache.AtlasMaterial;
            Renderer.Material = mat;

            RigidBody.UseGravity = true;
        }

        protected override void OnDelete()
        {
            Item = null;
            Renderer?.Delete();
            //Item?.Delete();
        }

        protected override void Update()
        {
            ChunkBoxCollisionProvider.CollideWorld(Collider);
        }

        public static ItemEntity FromItem(Item item, byte amount) => FromContainerItem(new ContainerItem(item, amount));

        public static ItemEntity FromContainerItem(ContainerItem item)
        {
            if(!item) throw new ArgumentNullException(nameof(item), "Unable to create an item entity from null !");
            if(item.Item == null) throw new ArgumentException($"{nameof(item)}.Item is null, unable to create an item entity from it !");
            if(item.Amount == 0) throw new ArgumentException($"{nameof(item)}.Amount is 0, unable to create an item entity from it !");
            
            WObject wobj = new WObject("Item Entity " + item);
            ItemEntity ent = wobj.AddModule<ItemEntity>();
            ent.Item = item;
            ent.Renderer.Mesh = item.Item.Model;
            return ent;
        }
    }
}