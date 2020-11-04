using System.Threading.Tasks;
using WEngine;
using WEngine.GUI;
using Winecrash.Entities;

namespace Winecrash.Client
{
    public class GameUI : Module
    {
        public WObject Crosshair;
        public Image HotbarImage;
        public WObject Hotbar;

        public static double UIScale { get; } = -0.05D;
        
        public static GameUI Instance { get; private set; }
        
        protected override void Creation()
        {
            if (Instance)
            {
                this.Delete();
                return;
            }

            Instance = this;
            
            Crosshair = new WObject("Crosshair")
            {
                Parent = Canvas.Main.WObject
            };
            Image img = Crosshair.AddModule<Image>();
            img.Picture = new Texture("assets/textures/crosshair.png");
            img.KeepRatio = true;
            img.MinAnchor = new Vector2D(0.49D, 0.49D);
            img.MaxAnchor = new Vector2D(0.51D, 0.51D);
            img.Material.SourceColorBlending = BlendingFactorSrc.OneMinusDstColor;
            
            
            Hotbar = new WObject("Hotbar")
            {
                Parent = Canvas.Main.WObject
            }; 
            HotbarImage = img = Hotbar.AddModule<Image>();
            img.Picture = new Texture("assets/textures/hotbar.png");
            img.KeepRatio = true;


            WObject rTester = new WObject("Render Tester")
            {
                Parent = Hotbar,
                LocalScale = Vector3D.One * 1.175D
            };
            Model mod = rTester.AddModule<Model>();
            mod.Renderer.Material = ItemCache.AtlasMaterial;
            mod.Renderer.Mesh = ItemCache.Get<Item>("winecrash:diamond_pickaxe").Model;
            mod.KeepRatio = true;
            mod.Depth = 1000;
            mod.MinAnchor = new Vector2D(3/182.0D,19/22.0D);
            mod.MaxAnchor = new Vector2D(19/182.0D,3/22.0D);


            
            WObject rTester2 = new WObject("Render Tester 2")
            {
                Parent = Hotbar,
                LocalScale = Vector3D.One * 1.175D
            };
            mod = rTester2.AddModule<Model>();
            mod.Renderer.Material = ItemCache.AtlasMaterial;
            mod.Renderer.Mesh = ItemCache.Get<Item>("winecrash:grass").Model;
            mod.KeepRatio = true;
            mod.Depth = 1000;
            mod.MinAnchor = new Vector2D(23/182.0D,19/22.0D);
            mod.MaxAnchor = new Vector2D(39/182.0D,3/22.0D);

            WObject rTester2TXT = new WObject("Amount")
            {
                Parent = rTester2
            };
            Label lb = rTester2TXT.AddModule<Label>();
            lb.Text = "64";
            lb.AutoSize = true;
            lb.Aligns = TextAligns.Down | TextAligns.Right;
            lb.MinAnchor = new Vector2D(0.05,-0.3);
            lb.MaxAnchor = new Vector2D(1.05,0.5);


        }
        
        private WObject itemPreview;
        protected override void Start()
        {
            Task.Run(() =>
            {
                Task.Delay(100).Wait();
                WObject handWobj = PlayerEntity.PlayerHandWobject;
                handWobj.LocalRotation = new Quaternion(0,90,0);
                handWobj.LocalRotation *= new Quaternion(0,0,-65);
                handWobj.LocalPosition = Vector3D.Forward * 0.9 + Vector3D.Left + Vector3D.Down * 0.6;
                MeshRenderer mr = handWobj.GetModule<MeshRenderer>();
                mr.Material = ItemCache.AtlasMaterial;
                mr.Mesh = ItemCache.Get<Item>("winecrash:diamond_pickaxe").Model;
            });
        }

        protected override void Update()
        {
        }

        protected override void OnRender()
        {
            HotbarImage.MinAnchor = new Vector2D(0.25D - UIScale, 0.0D);
            HotbarImage.MaxAnchor = new Vector2D(0.75D + UIScale, 1.0D);
            HotbarImage.Shift = Vector3D.Down * ((Graphics.Window.SurfaceResolution.Y / 2.0D) - HotbarImage.GlobalScale.Y / 2.0D);
        }

        protected override void OnEnable()
        {
            Crosshair.Enabled = true;
            Hotbar.Enabled = true;
        }

        protected override void OnDisable()
        {
            Crosshair.Enabled = false;
            Hotbar.Enabled = false;
        }

        protected override void OnDelete()
        {
            if (Instance == this) Instance = null;
            Crosshair?.Delete();
            Crosshair = null;

            HotbarImage = null;
            Hotbar?.Delete();
            Hotbar = null;
        }
    }
}