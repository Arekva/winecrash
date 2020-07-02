using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    public class Block : Item
    {
        public bool Transparent { get; set; } = false;

        public override void OnDeserialize()
        {

            base.OnDeserialize();
        }
    }
}
