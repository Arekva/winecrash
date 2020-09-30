using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using WEngine.Networking;

namespace Winecrash.Client
{
    public class ClientTester : Module
    {
        protected override void Update()
        {
            Program.Client?.ThreatData();
        }
    }
}
