using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    public abstract class Item : IEquatable<Item>
    {
        private short CacheIndex { get; set; } = 0;

        public string Identifier
        {
            get
            {
                return this.Definition.Identifier;
            }
        }

        internal protected ItemDef Definition
        {
            get
            {
                return ItemCache.Get(CacheIndex);
            }
        }

        public Item(string identifier)
        {
            this.CacheIndex = ItemCache.GetCacheIndex(identifier);
        }

        public bool Equals(Item other)
        {
            return other == null ? false : other.Identifier == this.Identifier;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Item);
        }

    }
}
