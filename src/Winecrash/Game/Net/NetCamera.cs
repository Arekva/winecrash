using Newtonsoft.Json;
using WEngine;
using WEngine.Networking;

namespace Winecrash.Net
{
    public class NetCamera : NetObject
    {
        public Vector2D Angles { get; set; }
        
        [JsonConstructor]
        public NetCamera(Vector2D angles)
        {
            this.Angles = angles;
        }
    }
}