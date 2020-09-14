using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WEngine.Networking
{
    public abstract class BaseClient : BaseObject
    {
        public TcpClient Client { get; private set; }

        public const int DefaultPort = 27716;
        
        public BaseClient(string host = "127.0.0.1", int port = DefaultPort)
        {
            Client = new TcpClient(host, port);
        }

        public void SendObject<T>(T netobj) where T : NetObject
        {
            NetObject.Send(netobj, Client.Client);
        }
    }
}
