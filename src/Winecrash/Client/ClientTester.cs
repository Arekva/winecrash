using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;

namespace Client
{
    public class ClientTester : Module
    {
        public double Cooldown { get; set; } = 1.0D;

        private double time { get; set; } = 0.0D;
        protected override void Update()
        {
            time -= Time.DeltaTime;

            if(time < 0.0 && Program.Client != null && Program.Client.Connected)
            {
                time = Cooldown;
                NetObject.Send(new NetPing(), Program.Client.Client.Client);
            }
        }
    }
}
