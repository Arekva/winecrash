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
    public delegate void PlayerConnectionDelegate(TcpPlayer player);
    public delegate void PlayerDisconnectionDelegate(TcpPlayer player, string reason);

    public class GameServer : BaseServer
    {
        public GameServer(IPAddress listenIp, int port) : base(listenIp, port) { }

        protected class RequiredAuth
        {
            public TcpClient Client { get; set; }
            public double CooldownTimeout { get; set; }

            public bool Deleted { get; private set; } = false;
            
            public void Delete()
            {
                if (Deleted) return;

                Client = null;
                Deleted = true;
            }
        }

        public event PlayerConnectionDelegate OnPlayerConnect;
        public event PlayerDisconnectionDelegate OnPlayerDisconnect;

        public List<TcpPlayer> ConnectedPlayers { get; set; } = new List<TcpPlayer>();
        public object ConnectedPlayersLocker = new object();

        protected List<RequiredAuth> AuthsRequired = new List<RequiredAuth>();
        protected object AuthLocker = new object();

        public double AuthTimeout = 5D;

        public override void Run()
        {
            base.Run();

            OnClientConnect += (client) =>
            {
                lock (AuthLocker)
                    AuthsRequired.Add(new RequiredAuth() { Client = client, CooldownTimeout = AuthTimeout });
                
                new NetPing().Send(client.Client);
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

            OnPlayerConnect += (player) =>
            {
                Debug.LogWarning(player.Nickname + " joined the game");
            };

            OnPlayerDisconnect += (player, reason) =>
            {
                Debug.LogWarning($"{player.Nickname} left the game");
            };
            
            Debug.Log("Winecrash " + Winecrash.Version + " - Server online.");
        }

        public TcpPlayer FindPlayer(TcpClient client)
        {
            TcpPlayer[] players = null;

            lock(ConnectedPlayersLocker)
            {
                players = ConnectedPlayers.ToArray();
            }

            return players.FirstOrDefault(p => p.Client == client);
        }

        public TcpPlayer FindPlayer(string nickname)
        {
            TcpPlayer[] players = null;

            lock (ConnectedPlayersLocker)
            {
                players = ConnectedPlayers.ToArray();
            }

            return players.FirstOrDefault(p => p.Nickname == nickname);
        }

        public override void Tick()
        {
            PendingData[] data;
            lock (this.PendingDataLocker)
            {
                data = this.PendingData.ToArray();
                this.PendingData.Clear();
            }
            
            
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Deleted) continue;

                if (data[i].NObject is NetPlayer nplayer)
                {
                    
                    RequiredAuth auth = null;
                    lock (AuthLocker)
                        auth = AuthsRequired.FirstOrDefault(a => a.Client == data[i].Client);

                    if(auth != null)
                    {
                        TcpPlayer player = new TcpPlayer(auth.Client, nplayer.Nickname);
                        lock (ConnectedPlayersLocker)
                            ConnectedPlayers.Add(player);
                        this.OnPlayerConnect?.Invoke(player);
                        lock (AuthLocker)
                            AuthsRequired.Remove(auth);
                        auth.Delete();
                    }
                }

                else if(data[i].NObject is NetEntity nentity)
                {
                    nentity.Parse();
                }

                data[i].Delete();
            }

            RequiredAuth[] rauths = null;
            lock(AuthLocker)
                rauths = AuthsRequired.ToArray();

            for(int i = 0; i < rauths.Length; i++)
            {
                rauths[i].CooldownTimeout -= Time.DeltaTime;
                if (rauths[i].CooldownTimeout < 0.0)
                {
                    DisconnectClient(rauths[i].Client, "Failed to authentify to the server");
                    
                    lock (AuthLocker)
                        AuthsRequired.Remove(rauths[i]);
                    rauths[i].Delete();
                }
            }

            //Debug.Log("NetObjects received within this tick: " + objects.Length);
            Engine.ForceUpdate();
        }

        protected override void DisconnectClient(TcpClient client, string reason)
        {
            TcpPlayer player = FindPlayer(client);
            if (player)
            {
                this.InvokeOnClientDisconnected(client, reason);
                this.OnPlayerDisconnect?.Invoke(player, reason);
                
                player.Kick(reason);

                lock (ConnectedPlayersLocker)
                    ConnectedPlayers.Remove(player);

                lock (ClientsLocker)
                    Clients.Remove(client);

                player.Delete();
            }

            else
            {
                Debug.LogWarning("Client " + client.Client.RemoteEndPoint.ToString() + " disconnected: " + reason.ToString());
                base.DisconnectClient(client, reason);
            }
        }
    }
}
