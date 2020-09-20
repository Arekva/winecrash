using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
