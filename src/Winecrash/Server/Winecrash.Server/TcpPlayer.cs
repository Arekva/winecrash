using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WEngine.Networking;

namespace Winecrash.Server
{
    public class TcpPlayer : Player
    {
        public TcpClient Client { get; }

        public bool Connected
        {
            get
            {
                return this.Client.Connected;
            }
        }

        public TcpPlayer(TcpClient client, string nickname) : base(nickname)
        {
            this.Client = client;
        }

        public void Kick(string reason)
        {
            NetObject.Send(new NetKick(reason), this.Client.Client);
            this.Client.Close();
        }

        public override void Delete()
        {
            this.Client.Dispose();
            base.Delete();
        }
    }
}
