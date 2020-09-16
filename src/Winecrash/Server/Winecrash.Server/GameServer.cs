using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;

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
                Task.Run(() => {
                //Debug.Log("something received");
                if (data is NetMessage msg)
                {
                    Debug.Log(client.Client.RemoteEndPoint.ToString() + " sent: " + msg.Message);
                }

                else if (data is NetPing ping)
                {
                    TimeSpan delta = ping.ReceiveTime - ping.SendTime;

                    Debug.Log("client -> server time: " + delta.Milliseconds + " ms");

                    NetObject.Send(ping, client.Client);
                }

                else
                {
                    Debug.Log("wtf has been received? " + data.GetType());
                }
                });
            };

            Debug.Log("Winecrash " + Game.Version + " - Server online.");
        }

        public override void Tick()
        {
            NetObject[] objects;
            lock (this.PendingDataLocker)
            {
                objects = this.PendingData.ToArray();
                this.PendingData.Clear();
            }

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].Delete();
            }

            //Debug.Log("NetObjects received within this tick: " + objects.Length);
            Engine.ForceUpdate();
        }
    }
}
