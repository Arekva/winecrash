using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.GUI;
using WEngine.Networking;
using Winecrash.Entities;
using Winecrash.Net;
using Graphics = WEngine.Graphics;

namespace Winecrash.Client
{
    public class PlayerController : Module
    {
        public static double MouseSensivity { get; set; } = 5.0D;

        private CameraMode _CameraMode { get; set; } = CameraMode.FPS;

        public static bool CameraLocked { get; set; } = false;


        public CameraMode CameraMode
        {
            get
            {
                return _CameraMode;
            }

            set
            {
                _CameraMode = value;

                switch(value)
                {
                    case CameraMode.FPS:
                        {
                            if(Player.LocalPlayer && Player.LocalPlayer.Entity && Player.LocalPlayer.Entity.ModelWObject) Player.LocalPlayer.Entity.ModelWObject.Enabled = false;
                            if(Player.LocalPlayer && Player.LocalPlayer.Entity && PlayerEntity.PlayerHandWobject) PlayerEntity.PlayerHandWobject.Enabled = true;

                            Camera.Main.WObject.Parent = this.WObject;
                            Camera.Main.WObject.LocalPosition = Vector3D.Up * EyeHeight;
                            CameraLocked = false;
                        }
                        break;

                    case CameraMode.TPSBehind:
                        {
                            Player.LocalPlayer.Entity.ModelWObject.Enabled = true;
                            PlayerEntity.PlayerHandWobject.Enabled = false;
                            Camera.Main.WObject.Parent = Player.LocalPlayer.Entity.PlayerHead;
                            Camera.Main.WObject.LocalPosition = /*Vector3D.Up * EyeHeight + */Vector3D.Backward * TPSMaxDistance;
                            Camera.Main.WObject.LocalRotation = new Quaternion(0, 0, 0);
                            CameraLocked = true;
                        }
                        break;

                    case CameraMode.TPSFront:
                        {
                            Player.LocalPlayer.Entity.ModelWObject.Enabled = true;
                            PlayerEntity.PlayerHandWobject.Enabled = false;
                            Camera.Main.WObject.Parent = Player.LocalPlayer.Entity.PlayerHead;
                            Camera.Main.WObject.LocalPosition = /*Vector3D.Up * EyeHeight + */Vector3D.Forward * TPSMaxDistance;
                            Camera.Main.WObject.LocalRotation = new Quaternion(0, 180, 0);
                            CameraLocked = true;
                        }
                        break;
                }
            }
        }

        public static double TPSMaxDistance { get; set; }= 3.5D;

        public static double EyeHeight { get; set; } = 1.62D;

        protected override void Creation()
        {
            base.Creation();
        }

        protected override void FirstFrame()
        {
            base.FirstFrame();

            CameraMode = CameraMode.FPS;
        }


        protected override void Update() //todo: NetworkUpdate
        {
            if (Player.LocalPlayer.Entity != null)
            {
                if(Input.IsPressing(GameInput.Key("View")))
                {
                    switch(CameraMode)
                    {
                        case CameraMode.FPS:
                            CameraMode = CameraMode.TPSBehind;
                            break;
                        case CameraMode.TPSBehind:
                            CameraMode = CameraMode.TPSFront;
                            break;
                        default:
                            CameraMode = CameraMode.FPS;
                            break;
                    }
                }


                Dictionary<string, KeyStates> inputs = new Dictionary<string, KeyStates>
                {
                    { "move_forward", Input.GetKeyState(GameInput.Key("Forward"), Input.KeyDictionary) },
                    { "move_backward", Input.GetKeyState(GameInput.Key("Backward"), Input.KeyDictionary) },
                    { "move_right", Input.GetKeyState(GameInput.Key("Right"), Input.KeyDictionary) },
                    { "move_left", Input.GetKeyState(GameInput.Key("Left"), Input.KeyDictionary) },
                    { "move_jump", Input.GetKeyState(GameInput.Key("Jump"), Input.KeyDictionary) },
                };


                NetInput ninput = new NetInput(inputs, Input.MouseDelta * Time.Delta * MouseSensivity);

                if (Input.IsPressing(Keys.Delete))
                {
                    Player.NoClipping = !Player.NoClipping;

                    if (Player.NoClipping)
                    {
                        Player.LocalPlayer.Entity.RigidBody.UseGravity = false;
                        Player.LocalPlayer.Entity.RigidBody.Velocity = Vector3D.Zero;
                    }
                    else
                    {
                        Player.LocalPlayer.Entity.RigidBody.UseGravity = true;
                        Player.LocalPlayer.Entity.RigidBody.Velocity = Vector3D.Zero;
                    }
                }
                

                Player.LocalPlayer.ParseInputs(inputs);
                Player.LocalPlayer.ParseMouse(Input.MouseDelta * Time.Delta * MouseSensivity);


                RaycastChunkHit hit = World.RaycastWorld(new Ray(Camera.Main.WObject.Position, Camera.Main.WObject.Forward, 5.0D));

                if (hit.HasHit)
                {
                    if (Input.IsPressing(Keys.MouseLeftButton))
                    {
                        Vector3I b = hit.LocalPosition;
                        hit.Chunk.Dig(b.X, b.Y, b.Z);
                        //Debug.Log(hit.Block.Identifier + " (Distance: " + hit.Distance + ")");
                    }
                    else if (Input.IsPressing(Keys.MouseRightButton))
                    {
                        Vector3D bg = hit.LocalPosition;
                        bg += (Vector3D)hit.Normal;
                        Vector3I b = bg;

                        Vector3I relb = World.RelativePositionToChunk(hit.LocalPosition, hit.Chunk.Coordinates);
                        
                        /*Chunk ec = hit.Chunk;
                        if (relb.Y < 0 || relb.Y > Chunk.Height - 1) // not in chunk ...
                        {
                            ec = null;
                        }

                        // x - z
                        if (ec != null)
                        {
                            if (relb.X < 0)
                            {
                                ec = World.GetChunk(ec.Coordinates + Vector2I.Left, "winecrash:overworld");
                                b.X = Chunk.Width - 1;
                            }

                            else if (relb.X > Chunk.Width - 1)
                            {
                                ec = World.GetChunk(ec.Coordinates + Vector2I.Right, "winecrash:overworld");
                                b.X = 0;
                            }
                            
                            if (relb.Z < 0)
                            {
                                ec = World.GetChunk(ec.Coordinates + Vector2I.Down, "winecrash:overworld");
                                b.Z = Chunk.Depth - 1;
                            }

                            else if (relb.Z > Chunk.Depth - 1)
                            {
                                ec = World.GetChunk(ec.Coordinates + Vector2I.Up, "winecrash:overworld");
                                b.Z = 0;
                            }
                        }*/

                        World.GlobalToLocal(hit.GlobalBlockPosition + (Vector3I)hit.Normal, out Vector2I cp, out Vector3I bp);

                        Player player = Player.LocalPlayer;
                        int selected = player.HotbarSelectedIndex;
                        ContainerItem ci = player.Hotbar[selected];

                        if (ci.Amount > 0 && ci.Item is Block block)
                        {
                            --ci.Amount;
                            Player.LocalPlayer.SetContainerItem(ci, selected);
                            World.GetChunk(cp, "winecrash:overworld")?.Edit(bp.X, bp.Y, bp.Z, block);
                        }

                        

                        //ec?.Edit(b.X, b.Y, b.Z, ItemCache.Get<Block>("winecrash:direction"));
                    }
                }

                World.GlobalToLocal(this.WObject.Position, out Vector2I global, out Vector3I local);

                //Debug.Log("world: " + this.WObject.Position.ToString("F2") + "       / " + "chunk: " + global + " / local: " + local);

                //#if DEBUG
                if (Input.IsPressing(Keys.NumPadAdd))
                {
                    //this place is meant to add a breakpoint in editors to see the current state of the game.
                    Debug.Log("Breaking program...");
                }

                if (Input.IsPressing(Keys.V))
                {
                    Graphics.Window.VSync = Graphics.Window.VSync == VSyncMode.Off ? VSyncMode.On : VSyncMode.Off;
                }

                if (Input.IsPressing(Keys.G))
                {
                    World.RebuildDimension("winecrash:overworld");
                }
                if (Input.IsPressing(Keys.O))
                {
                    Winecrash.RenderDistance--;
                }
                if (Input.IsPressing(Keys.P))
                {
                    Winecrash.RenderDistance++;
                }
                //#endif

                if (Program.Client.Connected)
                {
                    ninput.Send(Program.Client.Client.Client);
                }
                
                //ninput.Delete();

                //if (Input.IsPressing(GameInput.Key("Forward")) || Input.IsPressed(GameInput.Key("Forward")))
                //    NetEntity ne = new NetEntity(Player.LocalPlayer.Entity);
                // ik the player shouldn't send anything else as inputs, but for now it will be enough.
                // and yes I know you can add a playercontroller to any player entity and it would overwrite server's one. eh.
                //ne.Send(Program.Client.Client.Client);
                //Debug.Log(Player.LocalPlayer.Entity.WObject.Position);
                //Program.Client.SendObject(ne);
            }
        }
        
        //private object _FixedInputsLockers = new object();
        //private Dictionary<string, KeyStates> _FixedInputs;
        /*private bool jump = false;

        protected override void Update()
        {
            lock (_FixedInputsLockers)
            {
                if (_FixedInputs != null) Player.LocalPlayer.ParseInputs(_FixedInputs);
                if (jump)
                {
                    Player.LocalPlayer.Entity.RigidBody.Velocity += Vector3D.Up * 8.75;
                    jump = false;
                }
                //_FixedInputs = null;
            }
        }*/

        protected override void OnDelete()
        {
            base.OnDelete();
        }
    }
}
