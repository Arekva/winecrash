using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WEngine.Networking
{
    public struct PendingData
    {
        public TcpClient Client { get; private set; }
        public NetObject NObject { get; private set; }

        public bool Deleted { get; private set; }

        public PendingData(TcpClient client, NetObject obj)
        {
            this.Client = client;
            this.NObject = obj;
            this.Deleted = false;
        }

        public void Delete()
        {
            if (this.Deleted) return;

            this.Client = null;
            this.NObject.Delete();
            this.NObject = null;
            this.Deleted = true;
        }
    }
}
