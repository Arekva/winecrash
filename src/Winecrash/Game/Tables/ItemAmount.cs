using System;

namespace Winecrash
{
    [Serializable]
    public struct ItemAmount : IEquatable<ItemAmount>, IEquatable<ContainerItem>
    {
        private string _identifier;
        private byte _amount;
        
        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }
        public byte Amount
        {
            get => _amount;
            set => _amount = value;
        }

        public ItemAmount(string identifier, byte amount) => (_identifier, _amount) = (identifier, amount);

        public override bool Equals(object obj) => obj != null && obj is ItemAmount ia && Equals(ia) || obj is ContainerItem ci && Equals(ci);
        public bool Equals(ItemAmount o) =>
            Amount == 0 && o.Amount == 0 || 
            o.Amount >= Amount && o.Identifier == Identifier;
        public bool Equals(ContainerItem o) => o != null && o.Valid && o.Item.Identifier == Identifier && o.Amount == Amount;

        public static explicit operator ContainerItem(ItemAmount ia) => new ContainerItem(ItemCache.Get<Item>(ia.Identifier ?? "winecrash:air"), ia.Amount);

        public static explicit operator ItemAmount(ContainerItem ci)
        {
            if (ci) return new ItemAmount(ci.Item.Identifier, ci.Amount);
            else return new ItemAmount("winecrash:air", 0);
        }
    }
}