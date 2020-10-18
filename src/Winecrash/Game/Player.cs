using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;
using Winecrash.Entities;
using Winecrash.Net;
using WinecrashCore.Net;

namespace Winecrash
{
    public class Player : BaseObject
    {
        public static List<Player> Players { get; set; } = new List<Player>();
        public static object PlayersLocker { get; private set; } = new object();
        
        public static double WalkSpeed = 5.0D;
        
        public string Nickname { get; private set; }
        public Guid GUID { get; private set; }
        public Texture Skin { get; private set; }
        public string SkinAddress { get; private set; }
        
        public TcpClient Client { get; set; }
        public PlayerEntity Entity { get; set; }
        
        public bool IsLocal
        {
            get
            {
                return this == LocalPlayer;
            }
        }
        
        public static Player LocalPlayer { get; set; }
        
        public bool Connected
        {
            get
            {
                return this.Client.Connected;
            }
        }

        public Player(string nickname) : base(nickname)
        {
            this.Nickname = nickname;
            this.GUID = EGuid.UniqueFromString(nickname);
            this.Client = null;
            this.SkinAddress = null;
            this.Entity = null;
            
            lock(PlayersLocker)
                Players.Add(this);
        }

        public Player(string nickname, TcpClient client, Guid guid, string skinAddress = null) : base(nickname)
        {
            this.Nickname = nickname;
            this.Client = client;
            this.GUID = guid;
            this.SkinAddress = skinAddress;
            
            lock(PlayersLocker)
                Players.Add(this);
        }

        public void Kick(string reason)
        {
            new NetKick(reason).Send(this.Client.Client);
            this.Client.Close();
        }

        public static Player Find(Player player)
        {
            Player[] players;
            lock (PlayersLocker)
                players = Players.ToArray();

            return players.FirstOrDefault(p => p.Identifier == player.GUID);
        }
        
        public static Player Find(Guid guid)
        {
            Player[] players;
            lock (PlayersLocker)
                players = Players.ToArray();

            return players.FirstOrDefault(p => p.Identifier == guid);
        }
        
        public static Player Find(string nickname)
        {
            Player[] players;
            lock (PlayersLocker)
                players = Players.ToArray();

            return players.FirstOrDefault(p => p.Nickname == nickname);
        }
        
        public override void Delete()
        {
            lock(PlayersLocker)
                Players.Remove(this);
            
            this.Nickname = null;
            this.SkinAddress = null;
            this.Client.Dispose();
            this.Client = null;
            this.Entity?.Delete();

            if(Engine.DoGUI)
                this.Skin?.Delete();
            base.Delete();
        }

        public void CreateNonLocalElements()
        {
            MeshRenderer mr = this.Entity.WObject.AddModule<MeshRenderer>();
            mr.Material = Material.Find("Unlit");
            mr.Material.SetData("color", Texture.GetOrCreate("assets/textures/steve.png"));
            mr.Material.SetData("albedo", Color256.White);
            mr.Mesh = Mesh.LoadFile("assets/models/Player_Head.obj", MeshFormats.Wavefront);
        }

        public void ParseInputs(NetInput inputs)
        {
            this.ParseInputs(inputs.KeyStateses);
        }
        public void ParseInputs(Dictionary<string, KeyStates> ks)
        {
            Vector3D dir = new Vector3D();
            
            if(ks.TryGetValue("move_forward", out KeyStates state))
            {
                if(state == KeyStates.Pressing || state == KeyStates.Pressed)
                    dir += Vector3D.Forward;
            }
            if (ks.TryGetValue("move_backward", out state))
            {
                if(state == KeyStates.Pressing || state == KeyStates.Pressed)
                    dir += Vector3D.Backward;
            }
            if (ks.TryGetValue("move_left", out state))
            {
                if(state == KeyStates.Pressing || state == KeyStates.Pressed)
                    dir -= Vector3D.Left;
            }
            if (ks.TryGetValue("move_right", out state))
            {
                if(state == KeyStates.Pressing || state == KeyStates.Pressed)
                    dir -= Vector3D.Right;
            }

            Entity.WObject.Position += dir.Normalized * Time.FixedDeltaTime * WalkSpeed;
            //Debug.Log(Entity.WObject.Position);
        }

        public PlayerEntity CreateEntity(WObject parent)
        {
            if (Entity != null) return Entity;

            PlayerEntity pe = parent.AddOrGetModule<PlayerEntity>();
            pe.Identifier = GUID;
            this.Entity = pe;

            return pe;
        }
    }
}
