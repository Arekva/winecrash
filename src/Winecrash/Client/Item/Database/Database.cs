using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Winecrash.Game
{
    public class Database
    {
        public ItemDB[] Items { get; set; }

        public static Database Load(string path)
        {
            return JsonConvert.DeserializeObject<Database>(File.ReadAllText(path));
        }

        public Item[] ParseItems(bool addToCache = true)
        {
            Item[] parsedItems = new Item[Items.Length];

            for (short i = 0; i < Items.Length; i++)
            {
                parsedItems[i] = JsonConvert.DeserializeObject(File.ReadAllText(Items[i].Path), Items[i].RuntimeType) as Item;
                parsedItems[i].Identifier = Items[i].Identifier;
                parsedItems[i].OnDeserialize();
            }

            if (addToCache)
            {
                ItemCache.AddItems(parsedItems);
            }

            return parsedItems;
        }
    }
}
