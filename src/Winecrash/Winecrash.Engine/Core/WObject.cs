using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public sealed class WObject : BaseObject
    {
        public Int64[] dummy = new Int64[100_000_000];

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

        public T AddModule<T>() where T : Module
        {
            T mod = (T)Activator.CreateInstance(typeof(T));
            mod.Name = mod.GetType().Name;
            mod.WObject = this;
            this._Modules.Add(mod);
            mod.Creation();

            Debug.Log(mod.ToString());
            //System.Windows.Forms.MessageBox.Show(mod.ToString());

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

        public override void Delete()
        {
            for (int i = 0; i < _Modules.Count; i++)
            {
                _Modules[i].Delete();
            }

            _Modules.Clear();
            _Modules = null;
            _WObjects.Remove(this);

            base.Delete();
        }
    }
}
