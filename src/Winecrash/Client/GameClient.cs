using System;
using WEngine;
using WEngine.Networking;
using Winecrash.Net;

namespace Client
{
    public class GameClient : BaseClient
    {
        private bool firstPingReceived = false;
        public GameClient(string hostname, int port = BaseClient.DefaultPort) : base(hostname, port)
        {
            this.OnDisconnected += (reason) =>
            {
                Debug.LogWarning("Disconnected from server: " + reason);
            };

            NetPing.OnReceive += (data, type, connection) =>
            {
                if (firstPingReceived) return;

                firstPingReceived = true;
                
                NetObject.Send(new NetPlayer("Arthur"), this.Client.Client);
            };
        }
    }
}
