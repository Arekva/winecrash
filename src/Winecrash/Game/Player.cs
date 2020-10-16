using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;
using Winecrash.Entities;

namespace Winecrash
{
    public class Player : BaseObject
    {
        public string Nickname { get; private set; }
        public Guid GUID { get; private set; }
        public Texture Skin { get; private set; }
        public string SkinAddress { get; private set; }
        
        public TcpClient Client { get; private set; }
        public PlayerEntity Entity { get; private set; }
        
        public bool Connected
        {
            get
            {
                return this.Client.Connected;
            }
        }

        public Player(string nickname, TcpClient client, PlayerEntity entity, Guid guid, string skinAddress = null)
        {
            this.Nickname = nickname;
            this.Client = client;
            this.GUID = guid;
            this.SkinAddress = skinAddress;
            this.Entity = entity;
        }
        
        public void Kick(string reason)
        {
            new NetKick(reason).Send(this.Client.Client);
            this.Client.Close();
        }
        
        public override void Delete()
        {
            this.Nickname = null;
            this.SkinAddress = null;
            this.Client.Dispose();
            this.Client = null;
            this.Entity?.Delete();


            if(Engine.DoGUI)
                this.Skin?.Delete();
            base.Delete();
        }
    }
}
