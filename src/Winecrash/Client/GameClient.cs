using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WEngine;
using WEngine.Networking;
using Winecrash.Entities;
using Winecrash.Net;
using Debug = WEngine.Debug;

namespace Winecrash.Client
{
    public class GameClient : BaseClient
    {
        public static bool FirstPingReceived { get; set; } = false;
        
        public GameClient() : base()
        {
            Player.LocalPlayer.Client = this.Client;
            this.OnDisconnected += (reason) =>
            {
                WEngine.Debug.LogWarning("Disconnected from server: " + reason);
                this.Client.Client.Disconnect(true);
                FirstPingReceived = false;
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
            
            for (int i = 0; i < pending.Length; i++)
            {
                NetObject nobj = pending[i];

                if (nobj.GetType() == typeof(NetPing))
                {
                    if (!FirstPingReceived)
                    {
                        FirstPingReceived = true;

                        //send auth on first data received.

                        new NetPlayer(Player.LocalPlayer).Send(Client.Client);
                    }
                }
                
                else if (nobj is NetChunk nchunk)
                {
                    World.GetOrCreateChunk(nchunk);
                }
                
                else if (nobj is NetPlayer nplay)
                {
                    Player existingP = Player.Find(nplay.Nickname);

                    if (existingP == null)
                    {
                        PlayerEntity existingE = (PlayerEntity)Entity.Get(EGuid.UniqueFromString(nplay.Nickname));
                        existingP = new Player(nplay.Nickname);
                        
                        if (existingE != null)
                        {
                            existingP.Entity = existingE;
                        }
                        else
                        {
                            existingP.CreateEntity(new WObject(nplay.Nickname));
                        }
                        //existingP.CreateNonLocalElements();
                    }
                    
                    existingP.Entity.CreateModel();
                }
                
                else if (nobj is NetEntity nent)
                {
                    Entity ent = nent.Parse();
                }
                
                else if (nobj is NetCamera ncam)
                {
                    Player.LocalPlayer.CameraAngles = ncam.Angles;
                }

                pending[i].Delete();
            }
        }
    }
}
