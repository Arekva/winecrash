using System;
using Newtonsoft.Json;
using WEngine.Networking;

namespace Winecrash.Net
{
    public class NetPlayer : NetObject
    {
        public Guid GUID { get; set; }

        [JsonConstructor]
        public NetPlayer(Guid guid)
        {
            this.GUID = guid;
        }
    }
}
