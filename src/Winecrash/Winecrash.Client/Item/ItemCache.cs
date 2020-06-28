using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    internal static class ItemCache
    {
        private static ItemDef[] Items { get; set; }
        private static Dictionary<string, short> ItemsIndex;

        public static ItemDef Get(string identifier)
        {
            return Items[GetCacheIndex(identifier)];
        }
        internal static short GetCacheIndex(string identifier)
        {
            if (ItemsIndex.TryGetValue(identifier, out short index))
            {
                return index;
            }

            else
            {
                throw new ArgumentException($"No item corresponding to the identifier \"{identifier}\".", nameof(identifier));
            }
        }

        public static ItemDef[] GetItems() { return Items; }

        public static ItemDef Get(short cacheIndex)
        {
            return Items[cacheIndex];
        }

        internal static ItemDef[] LoadItems()
        {
            string path = "assets/block_db.txt";
            string[] ids = File.ReadAllLines(path);

            Items = new ItemDef[ids.Length];
            ItemsIndex = new Dictionary<string, short>(ids.Length);

            for (short i = 0; i < ids.Length; i++)
            {
                Items[i] = new ItemDef(ids[i], i);
                ItemsIndex.Add(ids[i], i);
            }

            return Items;
        }
    }
}
