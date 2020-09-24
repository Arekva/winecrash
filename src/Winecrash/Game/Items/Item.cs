using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash
{
    public abstract class Item : IEquatable<Item>
    {
        public string Identifier { get; internal set; } = "winecrash:air";

        public virtual void OnDeserialize() { }

        public bool Equals(Item other)
        {
            return other != null && other.Identifier == this.Identifier;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Item);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Identifier} ({this.GetType().Name})";
        }
    }
}
