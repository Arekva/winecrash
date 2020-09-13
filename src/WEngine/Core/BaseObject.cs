using System;

namespace WEngine
{
    /// <summary>
    /// The base object used by the engine. Mostly contains a GUID and can be deleted by properly handling objects for the GC.
    /// </summary>
    public abstract class BaseObject : IComparable
    {
        /// <summary>
        /// The name of this object.
        /// </summary>
        public string Name { get; set; } = "BaseObject";
        /// <summary>
        /// The global unique identifier of this object. Automatically generated.
        /// </summary>
        public Guid Identifier { get; }

        /// <summary>
        /// If this object is deleted.
        /// </summary>
        internal bool Deleted { get; private set; } = false;
        /// <summary>
        /// If this object is undeletable.
        /// </summary>
        public virtual bool Undeletable { get; internal set; } = false;

        /// <summary>
        /// Internal Enabled value. Preferably override the <see cref="SetEnable"/> function to set it.
        /// </summary>
        protected bool _Enabled = true;

        /// <summary>
        /// Is this object enabled?
        /// </summary>
        public virtual bool Enabled
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

        /// <summary>
        /// Create a new BaseObject, the <see cref="Identifier"/> is automatically generated.
        /// </summary>
        public BaseObject() 
        {
            this.Identifier = Guid.NewGuid();
        }

        /// <summary>
        /// Create a new BaseObject, the <see cref="Identifier"/> is automatically generated
        /// </summary>
        /// <param name="name">The object name.</param>
        public BaseObject(string name) : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Force the object to delete, even if <see cref="Undeletable"/> is set to <see cref="true"/>
        /// </summary>
        internal virtual void ForcedDelete() { }

        /// <summary>
        /// Set <see cref="Enabled"/>.
        /// </summary>
        /// <param name="status">The enabling status.</param>
        internal virtual void SetEnable(bool status) 
        {
            this._Enabled = status;
        }

        /// <summary>
        /// Clears object's data. It is very recommanded to override this to remove all other objects this <see cref="BaseObject"/> references.
        /// <br>Once used, also remove any reference other scripts has to it in order to free all the memory and truly delete it.</br>
        /// </summary>
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

        /// <summary>
        /// Determines if the object is the same as another one by their <see cref="Identifier"/>.
        /// </summary>
        /// <param name="obj">The object to compare this to.</param>
        /// <returns>If both objects are the same one.</returns>
        public override bool Equals(object obj)
        {
            return obj is BaseObject wobj ? this.Identifier == wobj.Identifier : false;
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        /// <summary>
        /// Get if an object is null.
        /// </summary>
        /// <param name="bobj">The object to check out.</param>
        public static implicit operator bool (BaseObject bobj)
        {
            return !(bobj is null);
        }
    }
}
