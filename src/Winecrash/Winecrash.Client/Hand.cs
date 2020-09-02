using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Game
{
    public class Hand : Module
    {
        protected override void OnRender()
        {
            this.WObject.Position = Camera.Main.WObject.Position + Camera.Main.WObject.Forward * 10;
        }
    }
}
