using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public abstract class BaseObject : IComparable
    {
        public string Name { get; set; }
        private Guid Identifier { get; }

        internal bool Deleted { get; private set; }

        public BaseObject() 
        {
            this.Identifier = Guid.NewGuid();
        }

        public BaseObject(string name)
        {
            this.Identifier = Guid.NewGuid();
            this.Name = name;
        }

        public virtual void Delete()
        {
            this.Deleted = true;
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(obj);
        }

        public override string ToString()
        {
            return $"{this.Name} ({this.GetType().Name})";
        }

        public override bool Equals(object obj)
        {
            return obj is BaseObject wobj ? this.Identifier == wobj.Identifier : false;
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }
    }
}
