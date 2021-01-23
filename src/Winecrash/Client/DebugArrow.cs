using WEngine;

namespace Winecrash.Client
{
    public class DebugArrow : Module
    {
        private WObject _arrow;
        protected override void Creation()
        {
            Mesh mesh = Mesh.LoadFile("assets/models/Arrow.obj");
            Shader shader = Shader.Find("Unlit");
            Material material = new Material(shader);
            Texture texture = new Texture("assets/textures/arrow.png");
            material.SetData("color", Color256.White);
            material.SetData("albedo", texture);
            material.SetData("tiling", Vector2D.One);

            WObject arrow = _arrow = new WObject("Arrow")
            {
                Parent = WObject.Find("Player Camera"),
                Layer = (ulong) Layers.Default, LocalPosition = Vector3D.Forward, LocalScale = Vector3D.One * 0.025 
            };
            MeshRenderer meshRenderer = arrow.AddModule<MeshRenderer>();
            meshRenderer.Material = material;
            meshRenderer.Mesh = mesh;
        }
        protected override void LateUpdate() => _arrow.Forward = Vector3D.Forward;
    }
}