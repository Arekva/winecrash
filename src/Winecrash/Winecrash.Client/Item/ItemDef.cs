using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    public class ItemDef
    {
        public string Identifier { get; private set; }

        internal short CacheIndex { get; private set; }

        internal ItemDef(string id, short cacheIndex)
        {
            this.Identifier = id;
            this.CacheIndex = cacheIndex;
        }

        public override string ToString()
        {
            return "ItemDef(" + Identifier + ")";
        }
    }
}
