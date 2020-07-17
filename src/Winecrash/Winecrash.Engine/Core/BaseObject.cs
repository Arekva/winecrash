using System;

namespace Winecrash.Engine
{
    public abstract class BaseObject : IComparable
    {
        public string Name { get; set; } = "BaseObject";
        public Guid Identifier { get; }

        internal bool Deleted { get; private set; } = false;

        public virtual bool Undeletable { get; internal set; } = false;

        private bool _Enabled = true;

        public bool Enabled
        {
            get
            {
                return this._Enabled;
            }

            set
            {
                this.SetEnable(value);
            }
        }

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

        internal virtual void SetEnable(bool status) 
        {
            this._Enabled = status;
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

        public static implicit operator bool (BaseObject bobj)
        {
            return !(bobj is null);
        }
    }
}
