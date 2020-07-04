using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Client
{
    public class Player : Module
    {
        public Camera FPSCamera;

        protected override void Creation()
        {
            this.FPSCamera = Camera.Main;
        }
    }
}
