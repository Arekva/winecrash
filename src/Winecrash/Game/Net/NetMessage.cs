using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine.Networking;

namespace Winecrash
{
    public class NetMessage : NetObject
    {

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }

            set
            {
                _Message = value;
            }
        }

        [JsonConstructor]
        public NetMessage(string message)
        {
            this._Message = message;
        }
    }
}
