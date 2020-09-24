using System;

namespace Winecrash
{
    public class ItemDB
    {
        public string Identifier { get; set; } = "winecrash:air";
        public Type RuntimeType { get; set; }
        public string Path { get; set; } = "assets/items/winecrash/air.json"; 
    }
}