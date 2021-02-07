using System;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using WEngine;

namespace Winecrash
{
    public class ContainerItem : BaseObject, IEquatable<ContainerItem>
    {
        /// <summary>
        /// The actual item reference stored
        /// </summary>
        public Item Item { get; set; } = null;

        //TODO: Use ulongs for AE-alike containers (storage servers 😏)
        /// <summary>
        /// The quantity of item into this slot
        /// </summary>
        public byte Amount { get; set; } = 1;

        public bool Valid => Item != null && Item.Identifier != "winecrash:air" && Amount > 0;

        public ContainerItem() : this("winecrash:air", 0) { }
        public ContainerItem(string identifier)
        {
            if (identifier == null) identifier = "winecrash:air";
            this.Item = ItemCache.Get<Item>(identifier);
        }
        public ContainerItem(Item item)
        {
            if (item == null) item = ItemCache.Get<Item>("winecrash:air");
            this.Item = item;
        }
        public ContainerItem(string identifier, byte amount) : this(identifier)
        {
            this.Amount = amount;
        }
        public ContainerItem(Item item, byte amount) : this(item)
        {
            this.Amount = amount;
        }

        public override bool Equals(object obj) => obj != null && obj is Container c && Equals(c);
        public bool Equals(ContainerItem other) => other != null && this.Item.Equals(other.Item) && Amount == other.Amount;

        public override string ToString() => (Item == null ? "Invalid" : this.Item.Identifier) + $" x{this.Amount}";

        public ContainerItem Duplicate() => new ContainerItem(Item, Amount);

        public override void Delete()
        {
            Item = null;
            Amount = 0;
            
            base.Delete();
        }
        
        

        
    }
}