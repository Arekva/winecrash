using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;

namespace Winecrash.Server
{
    public class GameServer : BaseServer
    {
        public GameServer(string listenIp = "0.0.0.0", int port = 27716) : base(listenIp, port) { }

        public override void Tick()
        {
            Engine.ForceUpdate();
        }
    }
}
