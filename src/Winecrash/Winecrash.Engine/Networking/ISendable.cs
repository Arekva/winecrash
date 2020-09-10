using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.Networking
{
    interface ISendable
    {
        public void Send(Socket socket);
    }
}
