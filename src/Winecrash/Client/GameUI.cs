﻿using WEngine;
using WEngine.GUI;

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
            
            
            Hotbar = new WObject("Crosshair")
            {
                Parent = Canvas.Main.WObject
            }; 
            HotbarImage = img = Hotbar.AddModule<Image>();
            img.Picture = new Texture("assets/textures/hotbar.png");
            img.KeepRatio = true;

            
            
            
            itemPreview = new WObject("item debug preview")
            {
                Position = Vector3D.Up * 80
                //Parent = Camera.Main.WObject,
                //LocalPosition = Vector3D.Forward * 10
            };

            MeshRenderer meshr = itemPreview.AddModule<MeshRenderer>();
            meshr.Material = ItemCache.AtlasMaterial;
            meshr.Mesh = ItemCache.Get<Item>("winecrash:diamond_pickaxe").Model;

        }
        
        private WObject itemPreview;

        protected override void Update()
        { 
            if (itemPreview)
            {
                itemPreview.LocalRotation *= new Quaternion(0, Time.DeltaTime * 10, 0);
            }
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