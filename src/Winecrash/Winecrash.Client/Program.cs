using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winecrash.Engine;
using System.Threading;
using OpenTK;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

using Winecrash.Engine.GUI;

using True = System.Boolean;

namespace Winecrash.Game
{
    static class Program
    {
        static void Main()
        {
            Task.Run(CreateDebugWindow);

            WEngine.Run().Wait();

            Graphics.Window.OnLoaded += Start;
        }

        static void Start()
        {
            Input.CursorVisible = true;
            Input.LockMode = CursorLockModes.Free;

            Input.MouseSensivity *= 2.5F;

            Physics.Gravity = new Vector3D(0, -27, 0); //-27

            Graphics.Window.VSync = Engine.VSyncMode.Off;

            Camera.Main.RenderLayers &= ~(1L << 32);
            Camera.Main.RenderLayers &= ~(1L << 48);

            MainMenu.Show();
        }
        public static void RunGameDebug()
        {
            MainMenu.Hide();

            Input.LockMode = CursorLockModes.Lock;
            Input.CursorVisible = false;

            Graphics.Window.Title = "test";

            new Shader("assets/shaders/cursor/Cursor.vert", "assets/shaders/cursor/Cursor.frag");
            WObject playerWobj = new WObject("Player");
            RigidBody rb = playerWobj.AddModule<RigidBody>();
            rb.UseGravity = false;
            BoxCollider bc = playerWobj.AddModule<BoxCollider>();

            bc.Extents = new Vector3D(0.4F, 0.9F, 0.4F);

            playerWobj.AddModule<Player>();

            //FreeCam.FreeCTRL = true;

            Camera.Main.WObject.AddModule<FreeCam>();

            Camera.Main._FarClip = 1000.0F;
            Camera.Main.FOV = 80.0F;

            playerWobj.Position = Vector3F.Up * 80F;

            Database db = Database.Load("assets/items/items.json");

            db.ParseItems();

            new Shader("assets/shaders/chunk/Chunk.vert", "assets/shaders/chunk/Chunk.frag");
            new Shader("assets/shaders/itemUnlit/ItemUnlit.vert", "assets/shaders/itemUnlit/ItemUnlit.frag");

            Chunk.ChunkTexture = ItemCache.BuildChunkTexture(out int xsize, out int ysize);
            Chunk.TexWidth = xsize;
            Chunk.TexHeight = ysize;
            CreateSkybox();

            WObject worldwobj = new WObject("World");
            worldwobj.AddModule<World>();


            new WObject("Debug").AddModule<DebugMenu>();
            new WObject("EscapeMenu").AddModule<EscapeMenu>();

            WObject crosshair = new WObject("Crosshair");
            crosshair.Parent = Engine.GUI.Canvas.Main.WObject;
            Engine.GUI.Image reticule = crosshair.AddModule<Engine.GUI.Image>();
            reticule.Picture = new Texture("assets/textures/crosshair.png");
            reticule.KeepRatio = true;
            reticule.Material.SourceColorBlending = OpenTK.Graphics.OpenGL4.BlendingFactorSrc.OneMinusDstColor;
            reticule.MinAnchor = new Vector2F(0.48F, 0.48F);
            reticule.MaxAnchor = new Vector2F(0.52F, 0.52F);

            reticule.MaxSize = Vector3F.One * 30.0F;

            WObject itembar = new WObject("Item Bar");
            itembar.Parent = Canvas.Main.WObject;
            Image bar = itembar.AddModule<Image>();
            bar.Picture = new Texture("assets/textures/itembar.png");
            bar.KeepRatio = true;
            bar.MinAnchor = new Vector2F(0.35F, 0.0F);
            bar.MaxAnchor = new Vector2F(0.65F, 0.08F);
            bar.Color = new Color256(1.0, 1.0, 1.0, 0.8F);
            
            Mesh mesh = Mesh.LoadFile("assets/models/BlockCube.obj", MeshFormats.Wavefront);
            for (int i = 0; i < ItemCache.TotalItems; i++)
            {
                Cube item = ItemCache.Get<Cube>(i);

                if (!(item is Cube)) continue;

                WObject cube = new WObject("Cube");

                cube.Parent = itembar;
                Model model = cube.AddModule<Model>();
                Material mat = new Material(Shader.Find("ItemUnlit"));
                mat.SetData<Vector2>("offset", new Vector2D(0, (double)i / ItemCache.TotalItems));
                mat.SetData<Vector2>("tiling", new Vector2D(1.0, 1.0 / ItemCache.TotalItems));
                mat.SetData<Vector3>("lightDir", new Vector3D(0.8, 1.0, -0.6));
                mat.SetData<Vector4>("ambiant", new Color256(0.0, 0.0, 0.0, 1.0));
                mat.SetData<Vector4>("lightColor", new Color256(2.0, 2.0, 2.0, 1.0));
                mat.SetData<Texture>("albedo", Texture.Find("Cache"));
                mat.SetData<Vector4>("color", Color256.White);

                model.Renderer.Mesh = mesh;
                model.Renderer.Material = mat;
                model.KeepRatio = true;

                cube.Scale *= 1.1F;
                cube.Rotation = new Engine.Quaternion(-21, 45, -20);

                float shift = i * 0.1093F;

                model.MinAnchor = new Vector2F(0.0175F + shift, 0.0F);
                model.MaxAnchor = new Vector2F(0.11F + shift, 1.0F);
            }

            WObject itemcursor = new WObject("Item Cursor");
            itemcursor.Parent = bar.WObject;
            Image itemcurs = itemcursor.AddModule<Image>();
            itemcurs.Picture = new Texture("assets/textures/barcursor.png");
            itemcurs.Color = new Color256(0.4, 0.4, 1.0, 1.0F);
            itemcurs.MinAnchor = new Vector2F(0.0F, 0.0F);
            itemcurs.MaxAnchor = new Vector2F(0.125F, 1.0F);
            itemcurs.KeepRatio = true;
        }
        

        static Color256 HorizonColourDay = new Color256(0.82D, 0.92D, 0.98D, 1.0D);
        static Color256 HorizonColourSunset = new Color256(1.0D, 0.48D, 0.0D, 1.0D);
        static Color256 NightColour = new Color256(0.0D, 0.0D, 0.0D, 1.0D);
        static Color256 HighAtmosphereColour = new Color256(0.23D, 0.41D, 0.70D, 1.0D);
        static Color256 GroundAtmosphereColour = new Color256(0.58D, 0.53D, 0.45D, 1.0D);

        static void CreateSkybox()
        {
            new Shader("assets/shaders/skybox/Skybox.vert", "assets/shaders/skybox/Skybox.frag");

            WObject sky = new WObject("Skybox");
            MeshRenderer mr = sky.AddModule<MeshRenderer>();

            sky.AddModule<DayNightCycle>();

            mr.Mesh = Mesh.LoadFile("assets/models/Skysphere.obj", MeshFormats.Wavefront);
            mr.UseMask = false;

            mr.Material = new Material(Shader.Find("Skybox"));
            mr.Material.SetData<Vector4>("horizonColourDay", HorizonColourDay);
            mr.Material.SetData<Vector4>("horizonColourSunset", HorizonColourSunset);
            mr.Material.SetData<Vector4>("nightColour", NightColour);
            mr.Material.SetData<Vector4>("highAtmosphereColour", HighAtmosphereColour);
            mr.Material.SetData<Vector4>("groundAtmosphereColour", GroundAtmosphereColour);

            mr.Material.SetData<float>("sunSize", 0.1F);

            mr.Material.SetData<Vector4>("sunInnerColor", new Color256(1.0D, 1.0D, 1.0D, 1.0D));
            mr.Material.SetData<Vector4>("sunOuterColor", new Color256(245D / 255D, 234D / 255D, 181D / 255D, 1.0D));

            sky.Layer = 1L << 32;

            WObject sun = new WObject("Sun");
            sun.AddModule<DirectionalLight>();
            

            SkyboxCamera skycam = new WObject("Skybox Camera").AddModule<SkyboxCamera>();
            skycam.ReferenceCamera = Camera.Main;
        }

        static void CreateDebugWindow()
        {
            Debug.AddLogger(new Logger(LogVerbose, LogWarning, LogError, LogException));
            Debug.Log("Winecrash Predev 0.2 - (C) Arthur Carré 2020");
        }

        static void LogVerbose(object msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg.ToString());
        }

        static void LogWarning(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg.ToString());
        }

        static void LogError(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg.ToString());
        }

        static void LogException(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg.ToString());
        }
    }
}
