using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    public class ItemDB
    {
        public string Identifier { get; set; } = "winecrash:air";
        public Type RuntimeType { get; set; }
        public string Path { get; set; } = "assets/items/winecrash/air.json";
    }
}
