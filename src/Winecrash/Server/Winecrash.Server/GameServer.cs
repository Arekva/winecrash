using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                Task.Run(async () =>
                {
                    try
                    {
                        Timer timer = new Timer((state) =>
                        {
                            throw new Exception();
                        }, null, 0, 500);

                        NetObject netobj = GetFirstObjectAsync(client);
                        if (netobj is NetPlayer player)
                        {
                            Debug.Log($"{player.Nickname} connected ({client.Client.RemoteEndPoint})");
                        }
                        else
                        {
                            Debug.LogWarning($"{client.Client.RemoteEndPoint} tried to connect but sent wrong infos (Execepted {typeof(NetPlayer)})");
                            DisconnectClient(client, $"Wrong connexion info provided (Execepted {typeof(NetPlayer)}).");
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.LogWarning($"{client.Client.RemoteEndPoint} tried to connect but sent no infos (Execepted {typeof(NetPlayer)})");
                        DisconnectClient(client, $"Wrong connexion info provided (Execepted {typeof(NetPlayer)}).");
                    }
                });
            };

            OnClientDisconnect += (client, reason) =>
            {
                Debug.LogWarning("Client " + client.Client.RemoteEndPoint.ToString() + " disconnected: " + reason.ToString());
            };

            OnClientDataReceived += (client, data) =>
            {
                Task.Run(() =>
                {
                    if (data is NetMessage msg)
                    {
                        Debug.Log(client.Client.RemoteEndPoint.ToString() + " sent: " + msg.Message);
                    }
                });
            };

            Debug.Log("Winecrash " + Game.Version + " - Server online.");
        }
        private async Task<NetObject> GetFirstObjectAsync(TcpClient client)
        {
            return await base.ReceiveDataAsync(client.Client);
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
