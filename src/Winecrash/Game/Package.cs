using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;
using Debug = WEngine.Debug;

namespace Winecrash
{
    public class Package
    {
        public ItemDB[] Items { get; set; }
        public string[] ManufactureTables { get; set; }
        public string[] HarvestTables { get; set; }
        

        public static Package Load(string path)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            Package pkg = JsonConvert.DeserializeObject<Package>(File.ReadAllText(path));

            pkg.ParseItems();
            pkg.ParseManufactureTables();
            pkg.ParseHarvestTables();
            
            sw.Stop();
            Debug.LogWarning($"Database loaded ({sw.Elapsed.TotalMilliseconds:F2} ms)");
            
            return pkg;
        }
        
        public HarvestTable[] ParseHarvestTables(bool addToCache = true)
        {
            HarvestTable[] parsedTables = new HarvestTable[HarvestTables.Length];

            Parallel.For(0, parsedTables.Length, i =>
            {
                parsedTables[i] = JsonConvert.DeserializeObject<HarvestTable>(File.ReadAllText(HarvestTables[i]));
            });

            if(addToCache) HarvestTable.Tables.AddRange(parsedTables);

            return parsedTables;
        }

        public ManufactureTable[] ParseManufactureTables(bool addToCache = true)
        {
            ManufactureTable[] parsedTables = new ManufactureTable[ManufactureTables.Length];

            //for (int i = 0; i < parsedTables.Length; i++) 
            Parallel.For(0, parsedTables.Length, i =>
            {
                parsedTables[i] = JsonConvert.DeserializeObject<ManufactureTable>(File.ReadAllText(ManufactureTables[i]));
            });

            if(addToCache) ManufactureTable.Tables.AddRange(parsedTables);

            return parsedTables;
        }

        public Item[] ParseItems(bool addToCache = true)
        {
            Item[] parsedItems = new Item[Items.Length];

            Parallel.For(0, parsedItems.Length, i =>
            {
                //for (short i = 0; i < Items.Length; i++)
                //{
                parsedItems[i] = JsonConvert.DeserializeObject(File.ReadAllText(Items[i].Path), Items[i].RuntimeType) as Item;
                parsedItems[i].Identifier = Items[i].Identifier;
                //}
            });
            

            if (addToCache) ItemCache.AddItems(parsedItems);

            Parallel.For(0, Items.Length, i =>
            {
                //for (short i = 0; i < Items.Length; i++)
                //{
                parsedItems[i].OnDeserialize();
                //}
            });
            
            return parsedItems;
        }
    }
}
