using System;
using WEngine;
using WEngine.Networking;
using Winecrash.Net;

namespace Client
{
    public class GameClient : BaseClient
    {
        public GameClient(string hostname, int port = BaseClient.DefaultPort) : base(hostname, port)
        {
            
            this.OnDisconnected += (reason) =>
            {
                Debug.LogWarning("Disconnected from server: " + reason);
            };
        }
    }
}
