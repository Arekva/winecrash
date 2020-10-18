using WEngine;

namespace Winecrash.Entities
{
    public class PlayerEntity : Entity
    {
        public WObject ModelWObject { get; set; }
        public WObject PlayerHead { get; set; }
        public static Mesh PlayerHeadMesh { get; set; }
        public static Texture DefaultTexture { get; set; }

        static PlayerEntity()
        {
            if (Engine.DoGUI)
            {
                PlayerHeadMesh = Mesh.LoadFile("assets/Models/Player_Head.obj", MeshFormats.Wavefront);
                
                DefaultTexture = new Texture("assets/textures/steve.png", "Steve", true);
            }
        }

        public void CreateModel()
        {
            ModelWObject = new WObject("Player Model"){ Parent = this.WObject };
            ModelWObject.LocalPosition = Vector3D.Zero;
            
            PlayerHead = new WObject("Head") { Parent = this.ModelWObject };
            PlayerHead.LocalPosition = Vector3D.Up * 1.62D;

            MeshRenderer mr = PlayerHead.AddModule<MeshRenderer>();
            mr.Mesh = PlayerHeadMesh;
            mr.Material = new Material(Shader.Find("Player"));
            mr.Material.SetData("albedo", DefaultTexture);
            mr.Material.SetData("color", Color256.White);
            mr.Material.SetData("tiling", Vector2D.One);
        }
        
        protected override void OnDelete()
        {
            ModelWObject?.Delete();
            
            base.OnDelete();
        }
    }
}