using System;
using WEngine;

namespace Winecrash.Client
{  
    public class SkyboxController : Module
    {
        static SkyboxController()
        {
            SunTexture = Texture.GetOrCreate("assets/textures/sun.png", true);
            MoonTexture = Texture.GetOrCreate("assets/textures/moon.png", true);
            SkyboxMesh = Mesh.LoadFile("assets/models/Skycube.obj", MeshFormats.Wavefront);
            
            SkyboxMaterial = new Material(Shader.Find("Skybox"));
        }

        public static Texture SunTexture { get; }
        public static Texture MoonTexture { get; }
        public static Mesh SkyboxMesh { get; }
        public static Material SkyboxMaterial { get; set; }
        
        public WObject MoonWobject { get; private set; }
        public WObject SunWobject { get; private set; }
        
        public WObject SkyWobjet { get; private set; }
        
        public MeshRenderer MainRenderer { get; private set; }
        
        public static SkyboxController Instance { get; private set; }
        
        public static Camera Camera { get; private set; }
        
        private double _Time = 0.0D;

        public double Time
        {
            get
            {
                return _Time;
            }

            set
            {
                _Time = value;
            }
        }

        protected override void Creation()
        {
            if (Instance != null)
            {
                this.Delete();
                return;
            }

            Instance = this;

            this.WObject.Layer = 1UL << 32;
            
            WObject camWobj = new WObject("Skybox Camera Wobject");
            Camera = camWobj.AddModule<Camera>();
            Camera.Depth = -100;
            Camera.RenderLayers = 1UL << 32;

            Camera.Main.RenderLayers &= ~(1UL << 32);

            SkyWobjet = new WObject("Sky") { Parent = this.WObject, Layer = 1UL << 32};

            MainRenderer = SkyWobjet.AddModule<MeshRenderer>();
            MainRenderer.UseDepth = false;
            MainRenderer.DrawOrder = 10000;

            MainRenderer.Material = SkyboxMaterial;
            MainRenderer.Mesh = SkyboxMesh;
            
            MainRenderer.Material.SetData("highColorDay", new Color32(122, 168, 255, 255));
            MainRenderer.Material.SetData("horizonColorDay", new Color32(201, 220, 244, 255));

            SunWobject = new WObject("Sun")
            {
                Parent = SkyWobjet,
                Layer = 1UL << 32,
                LocalScale = Vector3D.One / 2.0D,
                LocalPosition = Vector3D.Up,
                LocalRotation = new Quaternion(0, 0, 270)
            };

            MeshRenderer mr = SunWobject.AddModule<MeshRenderer>();
            mr.Material = new Material(Shader.Find("CelestialBody"));
            mr.Material.SetData("albedo", SunTexture);
            mr.Material.SetData("tiling", Vector2D.One);
            mr.Material.SetData("color", Color256.White);
            
            mr.Mesh = Mesh.LoadFile("assets/models/Quad.obj", MeshFormats.Wavefront);
            mr.UseDepth = false;
            mr.DrawOrder = 10001;
            
            
            MoonWobject = new WObject("Moon")
            {
                Parent = SkyWobjet,
                Layer = 1UL << 32,
                LocalScale = Vector3D.One / 2.0D,
                LocalPosition = Vector3D.Down,
                LocalRotation = new Quaternion(0, 0, 90)
            };
            mr = MoonWobject.AddModule<MeshRenderer>();
            mr.Material = new Material(Shader.Find("CelestialBody"));
            mr.Material.SetData("albedo", MoonTexture);
            mr.Material.SetData("tiling", Vector2D.One);
            mr.Material.SetData("color", Color256.White);
            
            mr.Mesh = Mesh.LoadFile("assets/models/Quad.obj", MeshFormats.Wavefront);
            mr.UseDepth = false;
            mr.DrawOrder = 10001;
            
            
            
            

            base.Creation();
        }
        

        protected override void Update()
        {
            //double sunAtmoPow = Vector3D.Dot(Vector3D.Up, SunWobject.LocalPosition.Normalized).Length;
            
            SunWobject?.GetModule<MeshRenderer>().Material.SetData("colorMult", new Color256(122/255.0D, 168/255.0D, 255/255.0D, 255/255.0D) * 1.0D);
            MoonWobject?.GetModule<MeshRenderer>().Material.SetData("colorMult", new Color256(122/255.0D, 168/255.0D, 255/255.0D, 255/255.0D) * 1.0D);
        }

        protected override void LateUpdate()
        {
            if (Camera.Main && Camera)
            {
                Camera.WObject.Rotation = Camera.Main.WObject.Rotation;
                Camera.FOV = Camera.Main.FOV;
            }
        }

        protected override void OnDelete()
        {
            Debug.Log("deleting :(");
            MainRenderer?.Delete();
            MainRenderer = null;
            
            SkyWobjet?.Delete();
            SkyWobjet = null;
            
            SunWobject?.Delete();
            SunWobject = null;
            
            MoonWobject?.Delete();
            MoonWobject = null;
            
            Camera?.Delete();
            Camera = null;

            Instance = null;

            base.OnDelete();
        }

        public void Hide()
        {
            if(Camera) Camera.Enabled = false;
            this.WObject.Enabled = false;
        }
        
        public void Show()
        {
            if(Camera) Camera.Enabled = true;
            this.WObject.Enabled = true;
        }
    }
}