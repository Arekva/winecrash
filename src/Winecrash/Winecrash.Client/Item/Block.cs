using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    public class Block : Item
    {
        public Block(string identifier) : base(identifier) { }

        public virtual void Tick() { }
    }
}
