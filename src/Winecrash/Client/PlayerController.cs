using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;
using Winecrash.Entities;
using Winecrash.Net;
using WinecrashCore.Net;

namespace Winecrash.Client
{
    public class PlayerController : Module
    {
        public static double WalkSpeed = 5.0D;
        protected override void FixedUpdate() //todo: NetworkUpdate
        {
            if (Player.LocalPlayer.Entity != null)
            {
                /*Vector3D dir = new Vector3D();
                if (Input.IsPressing(GameInput.Key("Forward")) || Input.IsPressed(GameInput.Key("Forward")))
                {
                    dir += Vector3D.Forward;
                }
                if (Input.IsPressing(GameInput.Key("Backward")) || Input.IsPressed(GameInput.Key("Backward")))
                {
                    dir += Vector3D.Backward;
                }
                if (Input.IsPressing(GameInput.Key("Right")) || Input.IsPressed(GameInput.Key("Right")))
                {
                    dir -= Vector3D.Right;
                }
                if (Input.IsPressing(GameInput.Key("Left")) || Input.IsPressed(GameInput.Key("Left")))
                {
                    dir -= Vector3D.Left;
                }

                Player.LocalPlayer.Entity.WObject.Position += dir.Normalized * Time.FixedDeltaTime * WalkSpeed;*/
                
                Dictionary<string, KeyStates> inputs = new Dictionary<string, KeyStates>();
                inputs.Add("move_forward", Input.GetKeyState(GameInput.Key("Forward"), Input.KeyDictionary));
                inputs.Add("move_backward", Input.GetKeyState(GameInput.Key("Backward"), Input.KeyDictionary));
                inputs.Add("move_right", Input.GetKeyState(GameInput.Key("Right"), Input.KeyDictionary));
                inputs.Add("move_left", Input.GetKeyState(GameInput.Key("Left"), Input.KeyDictionary));

                new NetInput(inputs).Send(Program.Client.Client.Client);
                
                Player.LocalPlayer.ParseInputs(inputs);
                
                //if (Input.IsPressing(GameInput.Key("Forward")) || Input.IsPressed(GameInput.Key("Forward")))
                //    NetEntity ne = new NetEntity(Player.LocalPlayer.Entity);
                // ik the player shouldn't send anything else as inputs, but for now it will be enough.
                // and yes I know you can add a playercontroller to any player entity and it would overwrite server's one. eh.
                //ne.Send(Program.Client.Client.Client);
                //Debug.Log(Player.LocalPlayer.Entity.WObject.Position);
                //Program.Client.SendObject(ne);
            }
        }

        protected override void OnDelete()
        {
            base.OnDelete();
        }
    }
}
