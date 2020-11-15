using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;
using Winecrash.Entities;
using Winecrash.Net;
using WinecrashCore.Net;

namespace Winecrash
{
    public partial class Player : BaseObject, IContainer
    {
        public static List<Player> Players { get; set; } = new List<Player>();
        public static object PlayersLocker { get; private set; } = new object();
        
        
        
        private Vector2D _CameraAngles = Vector2D.Zero;
        public static double MaxYAngles = 89.99D;

        public static double JumpTimer = 0.1D;
        public static double JumpForce = 5.0D;

        public static double WalkSpeed = 4.3D;
        public static double WalkAcceleration = 50.0D*25;
        public static double WalkDeaccelerationFactor = 16.0D;
        public static double StopSpeed = 0.05D;

        public Vector2D CameraAngles
        {
            get
            {
                return this._CameraAngles;
            }

            set
            {
                value.Y = WMath.Clamp(value.Y, -MaxYAngles, MaxYAngles);
                this._CameraAngles = value;

                if (this.Entity != null)
                {
                    this.Entity.Rotation = new Quaternion(-value.Y, value.X, 0);
                }
            }
        }

        public static bool NoClipping { get; set; } = false;
        
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
            
            OnItemAdd = null;
            OnItemRemove = null;
            OnItemUpdate = null;   
            OnHotbarUpdate = null;
            OnStorageUpdate = null;
            OnBagUpdate = null;
            OnArmorUpdate = null;
            OnCraftUpdate = null;
            OnCraftOutputUpdate = null;
            OnHotbarSelectedChange = null;
            OnContainerClose = null;
            OnContainerOpen = null;
            OnContainerToggle = null;

            
            base.Delete();
        }

        public void ForceCameraAngles(Vector2D angles)
        {
            this._CameraAngles = angles;
        }
        public void CreateNonLocalElements()
        {
            MeshRenderer mr = this.Entity.WObject.AddModule<MeshRenderer>();
            mr.Material = Material.Find("Unlit");
            mr.Material.SetData("color", Texture.GetOrCreate("assets/textures/steve.png"));
            mr.Material.SetData("albedo", Color256.White);
            mr.Mesh = Mesh.LoadFile("assets/models/Player_Head.obj", MeshFormats.Wavefront);
        }
        
        public void ParseNetInput(NetInput inputs)
        {
            this.ParseMouse(inputs.MouseDeltas);
            this.ParseInputs(inputs.KeyStateses);
        }
        public void ParseMouse(Vector2D deltas)
        {
            if (!ContainerOpened)
            {
                this.CameraAngles += deltas;
            }
        }
        public void ParseInputs(Dictionary<string, KeyStates> ks)
        {
            Vector3D dir = new Vector3D();

            Entity.AnyMoveInputOnFrame = false;
            if (!ContainerOpened)
            {
                if (ks.TryGetValue("move_forward", out KeyStates state))
                {
                    if (state == KeyStates.Pressing || state == KeyStates.Pressed)
                    {
                        dir += Vector3D.Forward;
                        Entity.AnyMoveInputOnFrame = true;
                    }

                }

                if (ks.TryGetValue("move_backward", out state))
                {
                    if (state == KeyStates.Pressing || state == KeyStates.Pressed)
                    {
                        dir += Vector3D.Backward;
                        Entity.AnyMoveInputOnFrame = true;
                    }
                }

                if (ks.TryGetValue("move_left", out state))
                {
                    if (state == KeyStates.Pressing || state == KeyStates.Pressed)
                    {
                        dir -= Vector3D.Left;
                        Entity.AnyMoveInputOnFrame = true;
                    }
                }

                if (ks.TryGetValue("move_right", out state))
                {
                    if (state == KeyStates.Pressing || state == KeyStates.Pressed)
                    {
                        dir -= Vector3D.Right;
                        Entity.AnyMoveInputOnFrame = true;
                    }
                }
            }

            /*if (ks.TryGetValue("move_jump", out state))
            {
                if (state == KeyStates.Pressing)
                {
                    this.Entity.RigidBody.Velocity += Vector3D.Up * 8.75;
                }
            }*/
            
            

            if(dir == Vector3D.Zero) Entity.AnyMoveInputOnFrame = false;

            if (NoClipping)
            {
                Entity.WObject.Position +=
                    this.Entity.Rotation * dir.Normalized * Time.FixedDeltaTime * WalkAcceleration * 0.1;
            }
            else
            {
                Entity.RigidBody.Velocity += (new Quaternion(0, CameraAngles.X, 0) * dir.Normalized) *
                                             Time.DeltaTime * WalkAcceleration;
            }
            
            //Debug.Log(Entity.RigidBody.Velocity);
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
