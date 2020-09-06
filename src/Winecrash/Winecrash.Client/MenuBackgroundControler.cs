using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Winecrash.Engine;
using Winecrash.Engine.GUI;


namespace Winecrash.Game
{
    public class MenuBackgroundControler : Module
    {
        public WObject EastPlane, WestPlane, UpPlane, DownPlane, ForwardPlane, BackwardPlane;

        public double RotationSpeed = 2.25D;
        public double Rotation = 0.0D;
        public Vector3D RotationAxis = Vector3D.Up;

        protected override void Creation()
        {
            Mesh quad = Mesh.LoadFile("assets/models/Quad.obj", MeshFormats.Wavefront);
            Shader shader = Shader.Find("Unlit");

            ForwardPlane = new WObject("Menu BG Forward Plane");
            ForwardPlane.Parent = this.WObject;
            ForwardPlane.LocalPosition = Vector3D.Forward * 0.5D;
            ForwardPlane.LocalRotation = new Quaternion(0, 90, 0);
            MeshRenderer mr = ForwardPlane.AddModule<MeshRenderer>();
            mr.Mesh = quad;
            mr.Material = new Material(shader);
            mr.Material.SetData<Texture>("albedo", new Texture("assets/textures/menu/+z.png", null, true));
            mr.Material.SetData<Color256>("color", Color256.White);
            mr.Material.SetData<Vector2D>("tiling", Vector2D.One);

            BackwardPlane = new WObject("Menu BG Backward Plane");
            BackwardPlane.Parent = this.WObject;
            BackwardPlane.LocalPosition = Vector3D.Backward * 0.5D;
            BackwardPlane.LocalRotation = new Quaternion(0, -90, 0);
            mr = BackwardPlane.AddModule<MeshRenderer>();
            mr.Mesh = quad;
            mr.Material = new Material(shader);
            mr.Material.SetData<Texture>("albedo", new Texture("assets/textures/menu/-z.png", null, true));
            mr.Material.SetData<Color256>("color", Color256.White);
            mr.Material.SetData<Vector2D>("tiling", Vector2D.One);


            UpPlane = new WObject("Menu BG Up Plane");
            UpPlane.Parent = this.WObject;
            UpPlane.LocalPosition = Vector3D.Up * 0.5D;
            UpPlane.LocalRotation = new Quaternion(0, 90, -90);
            mr = UpPlane.AddModule<MeshRenderer>();
            mr.Mesh = quad;
            mr.Material = new Material(Shader.Find("Unlit"));
            mr.Material.SetData<Texture>("albedo", new Texture("assets/textures/menu/+y.png", null, true));
            mr.Material.SetData<Color256>("color", Color256.White);
            mr.Material.SetData<Vector2D>("tiling", Vector2D.One);


            DownPlane = new WObject("Menu BG Down Plane");
            DownPlane.Parent = this.WObject;
            DownPlane.LocalPosition = Vector3D.Down * 0.5D;
            DownPlane.LocalRotation = new Quaternion(180, -90, -90);
            mr = DownPlane.AddModule<MeshRenderer>();
            mr.Mesh = quad;
            mr.Material = new Material(shader);
            mr.Material.SetData<Texture>("albedo", new Texture("assets/textures/menu/-y.png", null, true));
            mr.Material.SetData<Color256>("color", Color256.White);
            mr.Material.SetData<Vector2D>("tiling", Vector2D.One);

            EastPlane = new WObject("Menu BG East Plane");
            EastPlane.Parent = this.WObject;
            EastPlane.LocalPosition = Vector3D.Left * 0.5D;
            EastPlane.LocalRotation = new Quaternion(0, 0, 0);
            mr = EastPlane.AddModule<MeshRenderer>();
            mr.Mesh = quad;
            mr.Material = new Material(shader);
            mr.Material.SetData<Texture>("albedo", new Texture("assets/textures/menu/+x.png", null, true));
            mr.Material.SetData<Color256>("color", Color256.White);
            mr.Material.SetData<Vector2D>("tiling", Vector2D.One);

            WestPlane = new WObject("Menu BG West Plane");
            WestPlane.Parent = this.WObject;
            WestPlane.LocalPosition = Vector3D.Right * 0.5D;
            WestPlane.LocalRotation = new Quaternion(0, 180, 0);
            mr = WestPlane.AddModule<MeshRenderer>();
            mr.Mesh = quad;
            mr.Material = new Material(shader);
            mr.Material.SetData<Texture>("albedo", new Texture("assets/textures/menu/-x.png", null, true));
            mr.Material.SetData<Color256>("color", Color256.White);
            mr.Material.SetData<Vector2D>("tiling", Vector2D.One);
        }

        protected override void Update()
        {
            Rotation += Time.DeltaTime * RotationSpeed;
            this.WObject.LocalRotation = new Quaternion(RotationAxis, Rotation);
        }
    }
}
