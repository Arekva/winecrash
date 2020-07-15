using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winecrash.Engine;
using System.Threading;
using OpenTK;
using System.IO;

using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Winecrash.Client
{
    static class Program
    {
        static void Main()
        {
            Task.Run(CreateDebugWindow);

            WEngine.Run();

            //Block b = new Block();
            //b.Textures = new TexturePaths();
            //File.WriteAllText("debug.json", JsonConvert.SerializeObject(b, Formatting.Indented));

            Viewport.OnLoaded += Start;
        }

        static void Start()
        {
            Input.LockMode = CursorLockModes.Lock;
            Input.CursorVisible = false;

            Input.MouseSensivity *= 5.0F;

            Physics.Gravity = new Vector3D(0, -27, 0);

            Viewport.Instance.VSync = OpenTK.VSyncMode.Off;

            WObject playerWobj = new WObject("Player");
            RigidBody rb = playerWobj.AddModule<RigidBody>();
            rb.UseGravity = false;
            BoxCollider bc = playerWobj.AddModule<BoxCollider>();

            bc.Extents = new Vector3D(0.4F, 0.9F, 0.4F);

            //rb.UseGravity = false;
            playerWobj.AddModule<Player>();

            Camera.Main.WObject.AddModule<FreeCam>();
            Camera.Main.RenderLayers &= ~(1L << 32);
            //Camera.Main.WObject.AddModule<RigidBody>();

            Camera.Main._FarClip = 1000.0F;
            Camera.Main.FOV = 80.0F;

            playerWobj.Position = Vector3F.Up * 80F;

            Database db = Database.Load("assets/items/items.json");

            db.ParseItems();

            new Shader("assets/shaders/chunk/Chunk.vert", "assets/shaders/chunk/Chunk.frag");

            Chunk.ChunkTexture = ItemCache.BuildChunkTexture(out int xsize, out int ysize);
            Chunk.TexWidth = xsize;
            Chunk.TexHeight = ysize;
            CreateSkybox();

            WObject worldwobj = new WObject("World");
            worldwobj.AddModule<World>();


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

            mr.Mesh = Mesh.LoadFile("assets/models/Skysphere.obj", MeshFormats.Wavefront);
            mr.UseMask = false;

            mr.Material = new Material(Shader.Find("Skybox"));
            mr.Material.SetData<Vector4>("horizonColourDay", HorizonColourDay);
            mr.Material.SetData<Vector4>("horizonColourSunset", HorizonColourSunset);
            mr.Material.SetData<Vector4>("nightColour", NightColour);
            mr.Material.SetData<Vector4>("highAtmosphereColour", HighAtmosphereColour);
            mr.Material.SetData<Vector4>("groundAtmosphereColour", GroundAtmosphereColour);

            mr.Material.SetData<float>("sunSize", 0.025F);
            mr.Material.SetData<Vector4>("sunInnerColor", new Color256(1.0D, 1.0D, 1.0D, 1.0D));
            mr.Material.SetData<Vector4>("sunOuterColor", new Color256(245D / 255D, 234D / 255D, 181D / 255D, 1.0D));

            sky.Layer = 1L << 32;


            WObject sun = new WObject("Sun");
            sun.AddModule<DirectionalLight>();
            sun.LocalRotation = new Engine.Quaternion(90, 0, 0);
            sun.AddModule<DayNightCycle>();

            SkyboxCamera skycam = new WObject("Skybox Camera").AddModule<SkyboxCamera>();
            skycam.ReferenceCamera = Camera.Main;

        }



        [STAThread]
        static void CreateDebugWindow()
        {
            Debug.AddLogger(new Logger(LogVerbose, LogWarning, LogError, LogException));
            Debug.Log("Winecraft Predev 0.2 - (C) Arthur Carré 2020");
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
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(msg.ToString());
        }

        static void LogException(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg.ToString());
        }
    }
}
