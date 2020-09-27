using System;
using WEngine;
using WEngine.Networking;
using Winecrash.Net;

namespace Winecrash.Client
{
    public class GameClient : BaseClient
    {
        private bool _FirstPingReceived = false;
        public GameClient() : base()
        {
            this.OnDisconnected += (reason) =>
            {
                Debug.LogWarning("Disconnected from server: " + reason);
                this.Client.Client.Disconnect(true);
                _FirstPingReceived = false;
                World.WorldWObject?.Delete();
                MainMenu.Show();
                MainMenu.ShowDisconnection(reason);
            };

            NetPing.OnReceive += (data, type, connection) =>
            {
                if (_FirstPingReceived) return;
                _FirstPingReceived = true;
                
                NetObject.Send(new NetPlayer("Arthur"), this.Client.Client);
            };
        }
    }
}
