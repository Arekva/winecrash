using WEngine;

namespace Winecrash
{
    public class Tool : Item
    {
        public uint Durability { get; set; }
        
        public override Quaternion HandRotation { get; set; } = new Quaternion(0, -90, 0) * 
                                                                new Quaternion(0,0,-30);

        public override Vector3D HandPosition { get; set; } = Vector3D.Forward * 1.0 + Vector3D.Left + Vector3D.Down * 0.6;
    }
}