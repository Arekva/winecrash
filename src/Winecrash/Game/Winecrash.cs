using WEngine;

namespace Winecrash
{
    public static class Winecrash
    {
        public static uint RenderDistance { get; set; } = 2;
        public static WEngine.Version Version { get; } = new WEngine.Version(0, 0, 1, "Alpha");
        static Winecrash()
        {
            Physics.Gravity = Vector3D.Down * 27;
        }
    }
}