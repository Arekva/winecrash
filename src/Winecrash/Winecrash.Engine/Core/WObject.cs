using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public sealed class WObject : BaseObject
    {
        internal static List<WObject> _WObjects { get; set; } = new List<WObject>(1);
        internal List<Module> _Modules { get; set; } = new List<Module>(1);

        public WObject() : base() 
        {
            _WObjects.Add(this);
        }

        public WObject(string name) : base(name) 
        {
            _WObjects.Add(this);
        }


        private WObject _Parent = null;
        public WObject Parent
        {
            get
            {
                return this._Parent;
            }

            set
            {
                this._Parent = value;
            }
        }

        private List<WObject> _Children = new List<WObject>();
        public WObject[] Children
        {
            get
            {
                return _Children.ToArray();
            }
        }


        public Vector3D Position { get; set; } = Vector3D.Zero;
        public Vector3D LocalPosition
        {
            get
            {
                return !this._Parent ? this.Position : this._Parent.Position - this.Position;
            }

            set
            {
                this.Position = !this._Parent ? value : this._Parent.Position + value;
            }
        }

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public Vector3F Scale { get; set; } = Vector3F.One;





        public static WObject Find(string name)
        {
            return _WObjects.Find(w => w.Name == name);
        }

        public T AddModule<T>() where T : Module
        {
            T mod = (T)Activator.CreateInstance(typeof(T));

            if(mod.Undeletable && !this.Undeletable)
            {
                Debug.LogError("Cannot add undeletable module on deletable WObject !");

                Group.GetGroup(0).RemoveModule(mod);
                return null;
            }

            mod.Name = mod.GetType().Name;
            mod.WObject = this;

            this._Modules.Add(mod);

            mod.Creation();

            return mod;
        }

        public T GetModule<T>() where T : Module
        {
            foreach (Module mod in this._Modules) if (mod is T module) return module;
            return null;
        }

        public List<T> GetModules<T>() where T : Module
        {
            List<T> modules = new List<T>();

            foreach (Module mod in _Modules) if (mod is T module) modules.Add(module);

            if (modules.Count == 0) return null;
            else return modules;
        }

        public T AddOrGetModule<T>() where T : Module
        {
            return this.GetModule<T>() ?? this.AddModule<T>();
        }

        internal sealed override void SetEnable(bool status)
        {
            for (int i = 0; i < _Children.Count; i++)
                _Children[i].Enabled = status;

            if(status == true && this._Parent)
                this._Parent.Enabled = true;

            base.SetEnable(status);
        }

        internal sealed override void ForcedDelete()
        {
            for (int i = 0; i < _Modules.Count; i++)
            {
                _Modules[i].ForcedDelete();
            }

            _Modules.Clear();
            _Modules = null;
            _WObjects.Remove(this);

            base.Delete();
        }
        public sealed override void Delete()
        {
            if (this.Undeletable)
            {
                Debug.LogWarning("Unable to delete " + this + " : wobject undeletable.");
                return;
            }

            for (int i = 0; i < _Modules.Count; i++)
            {
                _Modules[i].Delete();
            } 

            for (int i = 0; i < _Children.Count; i++)
            {
                _Children[i].Delete();
            }

            this._Parent = null;
            _Children.Clear();
            _Children = null;

            _Modules.Clear();
            _Modules = null;
            _WObjects.Remove(this);

            base.Delete();
        }
    }
}
