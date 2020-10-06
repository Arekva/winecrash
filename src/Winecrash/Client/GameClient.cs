using System;
using System.Windows.Forms;
using WEngine;
using WEngine.Networking;
using Winecrash.Net;

namespace Winecrash.Client
{
    public class GameClient : BaseClient
    {
        public static bool FirstPingReceived { get; set; } = false;
        
        public string Nickname { get; set; } = null;
        public GameClient(string nickname) : base()
        {
            this.Nickname = nickname;
            this.OnDisconnected += (reason) =>
            {
                Debug.LogWarning("Disconnected from server: " + reason);
                this.Client.Client.Disconnect(true);
                FirstPingReceived = false;
                World.WorldWObject?.Delete();
                MainMenu.Show();
                MainMenu.ShowDisconnection(reason);

            };
        }

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

                        NetObject.Send(new NetPlayer(Nickname), Client.Client);
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
