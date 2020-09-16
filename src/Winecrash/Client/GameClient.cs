using System;
using WEngine;
using WEngine.Networking;

namespace Client
{
    public class GameClient : BaseClient
    {
        public GameClient(string hostname, int port = BaseClient.DefaultPort) : base(hostname, port)
        {
            
        }
    }
}
