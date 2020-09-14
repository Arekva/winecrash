using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;
using Winecrash.Net;

namespace Winecrash.Server
{
    public class GameServer : BaseServer
    {
        public GameServer(IPAddress listenIp, int port) : base(listenIp, port) { }

        public override void Run()
        {
            base.Run();

            OnClientConnect += (client) =>
            {
                Debug.LogWarning("Client connected from " + client.Client.RemoteEndPoint.ToString());
            };

            OnClientDisconnect += (client, reason) =>
            {
                Debug.LogWarning("Client " + client.Client.RemoteEndPoint.ToString() + " disconnected: " + reason.ToString());
            };

            OnClientDataReceived += (client, data) =>
            {
                Debug.Log("something received");
                /*if (data is NetMessage msg)
                {
                    Debug.Log(client + " sent: " + msg);
                }*/
            };

            Debug.Log("Winecrash " + Game.Version + " - Server online.");
        }

        public override void Tick()
        {
            Engine.ForceUpdate();
        }
    }
}
