using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;
using Winecrash.Engine.Networking;

namespace Winecrash.Server
{
    public class GameServer : BaseServer
    {
        public GameServer(string listenIp = "0.0.0.0", int port = 27716) : base(listenIp, port) { }

        int n = 0;
        public override void Tick()
        {
            Debug.Log("Server tick " + ++n);
        }
    }
}
