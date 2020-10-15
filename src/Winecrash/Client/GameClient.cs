using System;
using System.Diagnostics;
using System.Windows.Forms;
using WEngine;
using WEngine.Networking;
using Winecrash.Net;

namespace Winecrash.Client
{
    public class GameClient : BaseClient
    {
        public static bool FirstPingReceived { get; set; } = false;
        
        public Player Player { get; set; } = null;
        public GameClient(Player localPlayer) : base()
        {
            this.Player = localPlayer;
            this.OnDisconnected += (reason) =>
            {
                WEngine.Debug.LogWarning("Disconnected from server: " + reason);
                this.Client.Client.Disconnect(true);
                FirstPingReceived = false;
                World.WorldWObject?.Delete();
                MainMenu.Show();
                MainMenu.ShowDisconnection(reason);

            };
        }

        int n = 0;
        public void ThreatData()
        {
            
            NetObject[] pending = null;

            lock (this.PendingDataLocker)
            {
                pending = this.PendingData.ToArray();
                this.PendingData.Clear();
            }

            //Debug.Log(pending.Length);
            for (int i = 0; i < pending.Length; i++)
            {
                NetObject nobj = pending[i];

                if (nobj.GetType() == typeof(NetPing))
                {
                    if (!FirstPingReceived)
                    {
                        FirstPingReceived = true;

                        //send auth on first data received.

                        new NetPlayer(Player.Nickname).Send(Client.Client);
                    }
                }
                
                else if (nobj is NetChunk nchunk)
                {
                    World.GetOrCreateChunk(nchunk);
                }

                pending[i].Delete();
            }
        }
    }
}
