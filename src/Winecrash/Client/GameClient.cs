using System;
using WEngine;
using WEngine.Networking;
using Winecrash.Net;

namespace Winecrash.Client
{
    public class GameClient : BaseClient
    {
        private bool firstPingReceived = false;
        public GameClient() : base()
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
