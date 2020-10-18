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
using Winecrash.Entities;
using Winecrash.Net;
using WinecrashCore.Net;

namespace Winecrash.Server
{
    public delegate void PlayerConnectionDelegate(Player player);
    public delegate void PlayerDisconnectionDelegate(Player player, string reason);

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

        public List<Player> ConnectedPlayers { get; set; } = new List<Player>();
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

        public Player FindPlayer(TcpClient client)
        {
            Player[] players = null;

            lock(ConnectedPlayersLocker)
            {
                players = ConnectedPlayers.ToArray();
            }

            return players.FirstOrDefault(p => p.Client == client);
        }

        public Player FindPlayer(string nickname)
        {
            Player[] players = null;

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
                //Debug.Log("DATA");
                if (data[i].Deleted) continue;

                if (data[i].NObject is NetPlayer nplayer)
                {
                    
                    RequiredAuth auth = null;
                    lock (AuthLocker)
                        auth = AuthsRequired.FirstOrDefault(a => a.Client == data[i].Client);

                    if(auth != null)
                    {
                        Guid playerGuid = EGuid.UniqueFromString(nplayer.Nickname);
                        Debug.Log(nplayer.Nickname + " : " + playerGuid);
                        
                        WObject playerWobj = new WObject(nplayer.Nickname);
                        Player player = new Player(nplayer.Nickname, auth.Client, playerGuid, null);
                        player.CreateEntity(playerWobj);

                        lock (ConnectedPlayersLocker)
                        {
                            foreach (Player p in ConnectedPlayers)
                            {
                                //send all curent players to the new one
                                new NetPlayer(p).Send(player.Client.Client);
                                
                                //send the new player to the connected ones
                                new NetPlayer(player).Send(p.Client.Client);
                            }

                            ConnectedPlayers.Add(player);
                        }

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
                
                else if (data[i].NObject is NetInput ninput)
                {
                    Player p = this.FindPlayer(data[i].Client);
                    if (p != null)
                    {
                        p.ParseInputs(ninput);
                    }
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
                    DisconnectClient(rauths[i].Client, "Failed to auth to the server");
                    
                    lock (AuthLocker)
                        AuthsRequired.Remove(rauths[i]);
                    rauths[i].Delete();
                }
            }

            Entity[] entities;
            lock (Entity.EntitiesLocker)
            {
                entities = Entity.Entities.ToArray();
            }

            for (int i = 0; i < entities.Length; i++)
            {
                if(entities[i].Deleted) continue;
                SyncEntity(entities[i]);
            }

            //Debug.Log("NetObjects received within this tick: " + objects.Length);
            Engine.ForceUpdate();
        }

        public virtual void SyncEntity(Entity entity)
        {
            Socket[] clients = null;
            lock (ConnectedPlayersLocker)
            {
                clients = new Socket[ConnectedPlayers.Count];

                for (int i = 0; i < clients.Length; i++)
                {
                    clients[i] = ConnectedPlayers[i].Client.Client;
                }
            }

            new NetEntity(entity).Send(clients);
        }

        protected override void DisconnectClient(TcpClient client, string reason)
        {
            Player player = FindPlayer(client);
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
