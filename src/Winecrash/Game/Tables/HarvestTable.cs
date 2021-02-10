using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Winecrash
{
    [Serializable]
    public struct HarvestTable : IComparable<HarvestTable>
    {
        public static List<HarvestTable> Tables { get; internal set; } = new List<HarvestTable>();
        
        private string _blockIdentifier; // used as block serialization
        
        [IgnoreDataMember]
        private Block _block;
        [JsonIgnore]
        public Block Block
        {
            get => _block;
            set => _block = value;
        }

        private double _chance;
        public double Chance
        {
            get => _chance;
            set => _chance = value;
        }

        private ItemAmount[] _results;
        public ItemAmount[] Results
        {
            get => _results;
            set => _results = value;
        }

        [JsonConstructor]
        public HarvestTable(string block, double chance, ItemAmount[] results)
        {
            _blockIdentifier = block;
            _chance = chance;
            _results = results;

            if (!ItemCache.TryGet<Block>(block, out _block)) throw new ArgumentException($"No block corresponding to \"{block}\" existing !");
            _block.HarvestTables.Add(this);
            _block.HarvestTables.Sort();
        }
        
        public int CompareTo(HarvestTable other) => _chance.CompareTo(other._chance);
    }
}