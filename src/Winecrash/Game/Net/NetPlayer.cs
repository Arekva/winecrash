using System;
using Newtonsoft.Json;
using WEngine.Networking;

namespace Winecrash.Net
{
    public class NetPlayer : NetObject
    {
        public string Nickname { get; set; }
        //public Guid GUID { get; set; }

        [JsonConstructor]
        /*public NetPlayer(Guid guid)
        {
            this.GUID = guid;
        }*/

        public NetPlayer(string nickname)
        {
            this.Nickname = nickname;
        }
        
        public NetPlayer(Player player)
        {
            this.Nickname = player.Nickname;
        }
    }
}
