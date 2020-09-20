using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEngine.Networking
{
    public class NetKick : NetObject
    {
        public string Reason { get; set; }

        [JsonConstructor]
        public NetKick(string reason)
        {
            this.Reason = reason;
        }
    }
}
