using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;

namespace Winecrash
{
    public class Database
    {
        public ItemDB[] Items { get; set; }
        
        public string[] ManufactureTables { get; set; }

        public static Database Load(string path)
        {
            Database db = JsonConvert.DeserializeObject<Database>(File.ReadAllText(path));

            db.ParseItems();
            db.ParseManufactureTables();
            
            return db;
        }

        public ManufactureTable[] ParseManufactureTables(bool addToCache = true)
        {
            ManufactureTable[] parsedTables = new ManufactureTable[ManufactureTables.Length];

            for (int i = 0; i < parsedTables.Length; i++)
            {
                ManufactureTable table = JsonConvert.DeserializeObject<ManufactureTable>(File.ReadAllText(ManufactureTables[i]));
                parsedTables[i] = table;
                if(addToCache) ManufactureTable.Cache.Add(table);
            }

            return parsedTables;
        }

        public Item[] ParseItems(bool addToCache = true)
        {
            Item[] parsedItems = new Item[Items.Length];
            
            for (short i = 0; i < Items.Length; i++)
            {
                parsedItems[i] = JsonConvert.DeserializeObject(File.ReadAllText(Items[i].Path), Items[i].RuntimeType) as Item;
                parsedItems[i].Identifier = Items[i].Identifier;
            }

            if (addToCache) ItemCache.AddItems(parsedItems);

            for (short i = 0; i < Items.Length; i++) parsedItems[i].OnDeserialize();

            return parsedItems;
        }
    }
}
