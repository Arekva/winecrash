using System;

namespace Winecrash.Engine
{
    public abstract class BaseObject : IComparable
    {
        public string Name { get; set; } = "BaseObject";
        private Guid Identifier { get; }

        internal bool Deleted { get; private set; } = false;

        public virtual bool Undeletable { get; internal set; } = false;

        public bool Enabled { get; set; } = true;

        public BaseObject() 
        {
            this.Identifier = Guid.NewGuid();
        }

        public BaseObject(string name)
        {
            this.Identifier = Guid.NewGuid();
            this.Name = name;
        }

        internal virtual void ForcedDelete() { }

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

        public static implicit operator bool (BaseObject bobj)
        {
            return !(bobj is null);
        }
    }
}
