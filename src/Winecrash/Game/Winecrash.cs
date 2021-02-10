using WEngine;

namespace Winecrash
{
    public static class Winecrash
    {
        private static uint _renderDistance = 5;

        public static readonly WRandom Random = new WRandom("minecrash".GetHashCode());
        
        public static Save CurrentSave { get; set; }

        public static uint RenderDistance
        {
            get => _renderDistance;
            set
            {
                _renderDistance = value;
                Material.Find("Item")?.SetData("renderDistance", (float)value);
            }
        }
        public static Version Version { get; } = new Version(0, 0, 0, "Alpha \"Stève\"");
        static Winecrash() => Physics.Gravity = Vector3D.Down * 27;
    }
}