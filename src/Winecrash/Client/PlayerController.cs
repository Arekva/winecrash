using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;
using Winecrash.Entities;
using Winecrash.Net;

namespace Winecrash.Client
{
    public class PlayerController : Module
    {
        public static PlayerEntity LocalPlayer { get; set; }

        protected override void FixedUpdate() //todo: NetworkUpdate
        {
            if (LocalPlayer != null)
            {
                NetEntity ne = new NetEntity(LocalPlayer);

                ne.Send(Program.Client.Client.Client);
                //Program.Client.SendObject(ne);
            }
        }

        protected override void OnDelete()
        {
            LocalPlayer = null;

            base.OnDelete();
        }
    }
}
