using System;
using WEngine;

namespace Winecrash.Entities
{
    public class ItemEntity : Entity
    {
        public ContainerItem Item { get; set; }
        public WObject RendererWObject { get; set; }
        public MeshRenderer Renderer { get; set; }
        
        protected override void Creation()
        {
            base.Creation();

            Vector3D scale = Vector3D.One * 0.25D;
            //WObject.LocalScale = scale; 
            Collider.Extents = scale / 2.0D;
            Collider.CastExtents = 0.75D;
            Collider.CastLength = 0.05D;

            RendererWObject = new WObject("Renderer")
            {
                Parent = this.WObject,
                LocalPosition = Vector3D.Zero,
                LocalScale = scale,
                LocalRotation = Quaternion.Identity
            };

            Renderer = this.RendererWObject.AddModule<MeshRenderer>();
            Material mat = ItemCache.AtlasMaterial;
            Renderer.Material = mat;

            RigidBody.UseGravity = true;
        }

        protected override void OnDelete()
        {
            base.OnDelete();
            
            Item = null;
            Renderer?.Delete();
            Renderer = null;
            RendererWObject = null;
            //Item?.Delete();
        }

        protected override void Update()
        {
            ChunkBoxCollisionProvider.CollideWorld(Collider);

            if (IsPicked)
            {
                PickTimer -= Time.Delta;
                PickMove();
            }

            
            
            if(Engine.DoGUI) Animate();

            if (PickTimer <= 0.0D)
            {
                FinishPicking();
            }
        }

        public static double AnimationBaseHeight { get; set; } = 0.2D;
        public static double AnimationMaxHeight { get; set; } = 0.4D;
        public static double AnimationHeightSpeed { get; set; } = 1.8D;

        public static double RotationSpeed { get; set; } = (360.0D / 4.0D) / AnimationHeightSpeed;

        public double TimeSinceSpawn { get; private set; } = 0.0D;
        public double PickableTime { get; private set; } = 0.75D;

        
        private double _pickTimerStart = 0.15D;
        private double PickTimer { get; set; } = 0.15D;
        private Vector3D PickEndScale { get; set; } = Vector3D.One*0.05D;
        
        public Player PickingPlayer { get; set; } = null;

        private bool _isPicked = false;
        private Vector3D _basePickPos;

        public bool IsPicked
        {
            get => _isPicked;
            set
            {
                _isPicked = value;
                _basePickPos = WObject.Position;
                RigidBody.Enabled = Collider.Enabled = !value;
            }
        }

        public bool IsPickable => !IsPicked && TimeSinceSpawn >= PickableTime;

        private void PickMove()
        {
            double t = 1.0D-(PickTimer / _pickTimerStart);
            this.WObject.Position = Vector3D.Lerp(_basePickPos, PickingPlayer.PickPositionAnimation, t);
            this.WObject.Scale = Vector3D.Lerp(Vector3D.One, PickEndScale, t);
        }
        private void Animate()
        {
            TimeSinceSpawn += Time.Delta;
            
            double height = WMath.Remap(Math.Cos(TimeSinceSpawn * AnimationHeightSpeed), -1.0D, 1.0D, AnimationBaseHeight, AnimationMaxHeight);
            Vector3D localPos = Vector3D.Up * height;
            Quaternion localRot = new Quaternion(0, RotationSpeed*TimeSinceSpawn, 0);
            RendererWObject.LocalPosition = localPos;
            RendererWObject.LocalRotation = localRot;
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

        public void FinishPicking()
        {
            if(!PickingPlayer) Debug.LogWarning("Unable to pick item: no player to do so.");
           
            PickingPlayer.AddItemFast(this.Item);
                
            this.WObject.Delete();
        }
    }
}