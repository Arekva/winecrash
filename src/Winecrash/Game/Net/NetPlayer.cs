using Newtonsoft.Json;
using WEngine.Networking;

namespace Winecrash.Net
{
    public class NetPlayer : NetObject
    {
        public string Nickname { get; set; }

        [JsonConstructor]
        public NetPlayer(string nickname)
        {
            this.Nickname = nickname;
        }
    }
}
