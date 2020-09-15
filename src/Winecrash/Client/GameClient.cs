using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEngine.Networking;
using Winecrash.Net;

namespace Client
{
    public class GameClient : BaseClient
    {
        public GameClient(string hostname, int port = BaseClient.DefaultPort) : base(hostname, port) 
        {
            
        }
    }
}
