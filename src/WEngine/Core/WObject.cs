using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace WEngine
{
    /// <summary>
    /// WObject are at the base of the game logic, they can hold multiple <see cref="Module"/> allowing scripting.
    /// <br>To add a module, use <see cref="WObject.AddModule{T}"/>. Remove and find also have their own methods.</br>
    /// </summary>
    public class WObject : BaseObject
    {
        internal static object WobjectsLocker = new object();
        // all the loaded wobjects
        internal static List<WObject> WObjects { get; set; } = new List<WObject>(1);
        // modules linked to this wobject
        internal List<Module> _Modules { get; set; } = new List<Module>(1);

        /// <summary>
        /// Create an empty <see cref="WObject"/>
        /// </summary>
        public WObject() : base()
        {
            lock(WobjectsLocker)
                WObjects.Add(this);
        }

        /// <summary>
        /// Create an empty <see cref="WObject"/>
        /// </summary>
        /// <param name="name">The name of the WObject.</param>
        public WObject(string name) : base(name) 
        {
            lock(WobjectsLocker)
                WObjects.Add(this);
        }

        /// <summary>
        /// Render layer of the WObject. The render layer
        /// 0 = none
        /// Int64.MaxValue = everything
        /// 
        /// ex: 1<<32 = skybox
        /// </summary>
        public UInt64 Layer { get; set; } = 1; // Default layer = 1;



        internal static ManualResetEvent GlobalTransformLocker = new ManualResetEvent(true);
        

        private WObject _Parent = null;
        public object ParentLocker = new object();
        public WObject Parent
        {
            get
            {
                //lock(ParentLocker)
                    return this._Parent;
            }

            set
            {
                if (value != null && this.Deleted)
                {
                    Debug.LogWarning( $"Unable to set a parent to \"{this.Name}\": it is deleted. Consider setting it to null everywhere.");
                }

                if(_Parent != null)
                {
                    lock (ParentLocker)
                    {
                        lock (_Parent._ChildrenLocker)
                            _Parent._Children.Remove(this);
                        _Parent = null;
                    }
                }

                if(value != null)
                {
                    if (value.Deleted)
                    {
                        Debug.LogWarning($"Unable to set \"{value.Name}\" as a parent for \"{this.Name}\": parent is deleted ! Consider setting it to null everywhere.");
                        return;
                    }

                    //value.Enabled = this.Enabled;
                    
                    Vector3F oldGlobalPosition = this.Position;
                    Quaternion oldGlobalRotation = this.Rotation;
                    Vector3F oldGlobalScale = this.Scale;

                    lock(ParentLocker)
                        this._Parent = value;

                    lock (value._ChildrenLocker)
                        value._Children.Add(this);

                    this.Position = oldGlobalPosition;
                    this.Rotation = oldGlobalRotation;
                    this.Scale = oldGlobalScale;
                }
            }
        }

        private List<WObject> _Children { get; set; } = new List<WObject>();
        private object _ChildrenLocker { get; set; } = new object();
        public WObject[] Children
        {
            get
            {
                return _Children.ToArray();
            }
        }

        private object _PositionLocker = new object();
        private Vector3D _Position;
        public Vector3D Position
        {
            get
            {
                lock (_PositionLocker)
                {
                    if (_PositionNeedsUpdate)
                    {
                        _PositionNeedsUpdate = false;
                        lock (ParentLocker)
                            //                                      first set to scale                                     then rotate                          then add parent's position
                            this._Position = this._Parent ? (this.LocalPosition * this._Parent.Scale).RotateAround(Vector3D.Zero, this._Parent.Rotation) + this._Parent.Position : this.LocalPosition;
                    }
                }

                return _Position;
                
            }

            set
            {
                lock (_PositionLocker)
                {
                    lock (ParentLocker)
                    {
                        //                                      first get relative position                              then rotate by invert                                 then scale
                        this.LocalPosition = this._Parent
                            ? (value - this._Parent.Position).RotateAround(Vector3D.Zero, this._Parent.Rotation) *
                              (Vector3D.One / this._Parent.Scale)
                            : value;
                    }

                    this._Position = value;
                    _PositionNeedsUpdate = false;

                    FlagTransformUpdatesRecursive(true, true, true, false, false);
                }
            }
        }
        private Vector3D _LocalPosition = Vector3D.Zero;
        public Vector3D LocalPosition
        {
            get
            {
                return this._LocalPosition;
            }
            set
            {
                lock (_PositionLocker)
                {
                    this._LocalPosition = value;
                    FlagTransformUpdatesRecursive(true, false, true, true, true);
                }
            }
        }

        private object _RotationLocker = new object();
        private Quaternion _Rotation;
        public Quaternion Rotation
        {
            get
            {
                lock (_RotationLocker)
                {
                    if (_RotationNeedsUpdate)
                    {
                        _RotationNeedsUpdate = false;
                        lock (ParentLocker)
                            _Rotation = this._Parent ? this._Parent.Rotation * this.LocalRotation : this.LocalRotation;
                    }
                }

                return _Rotation;
                
            }

            set
            {
                lock (_RotationLocker)
                {
                    lock (ParentLocker)
                        this.LocalRotation = this._Parent ? this._Parent.Rotation.Inverted * value : value;

                    this._Rotation = value;
                    _RotationNeedsUpdate = false;

                    FlagTransformUpdatesRecursive(true, true, true, true, false);
                }
            }
        }

        
        private Quaternion _LocalRotation = Quaternion.Identity;
        public Quaternion LocalRotation
        {
            get
            {
                return _LocalRotation;
            }

            set
            {
                lock (_RotationLocker)
                {
                    this._LocalRotation = value;
                    FlagTransformUpdatesRecursive(true, false, true, true, false);
                }
            }
        }
        
        private object _ScaleLocker = new object();
        
        private Vector3D _Scale;
        public Vector3D Scale
        {
            get
            {
                lock (_ScaleLocker)
                {
                    if (_ScaleNeedsUpdate)
                    {
                        _ScaleNeedsUpdate = false;
                        lock (ParentLocker)
                            _Scale = this._Parent ? this._Parent.Scale * this.LocalScale : this.LocalScale;
                    }

                    return _Scale;
                }
            }

            set
            {
                lock (_ScaleLocker)
                {
                    lock (ParentLocker)
                        this.LocalScale = this._Parent ? value / this._Parent.Scale : value;

                    this._Scale = value;
                    _ScaleNeedsUpdate = false;


                    FlagTransformUpdatesRecursive(true, true, true, false, true);
                }
            }
        }
        private Vector3D _LocalScale = Vector3D.One;
        public Vector3D LocalScale
        {
            get
            {
                return this._LocalScale;
            }
            set
            {
                lock (_ScaleLocker)
                {
                    this._LocalScale = value;
                    FlagTransformUpdatesRecursive(true, false, true, false, true);
                }
            }
        }

        private bool _PositionNeedsUpdate = true;
        private bool _RotationNeedsUpdate = true;
        private bool _ScaleNeedsUpdate = true;


        private bool _RightNeedsUpdate = true;
        private bool _LeftNeedsUpdate = true;
        private bool _UpNeedsUpdate = true;
        private bool _DownNeedsUpdate = true;
        private bool _ForwardNeedsUpdate = true;
        private bool _BackwardNeedsUpdate = true;

        private void FlagTransformUpdatesRecursive(bool value, bool excludeThis, bool doPosition, bool doRotation, bool doScale)
        {
            this.FlagDirectionUpdates(value);
            if (doPosition && !excludeThis) this._PositionNeedsUpdate = true;
            if (doRotation && !excludeThis) this._RotationNeedsUpdate = true;
            if (doScale && !excludeThis) this._ScaleNeedsUpdate = true;


            //this.FlagTransformUpdates(value);

            WObject[] children = null;
            lock (_ChildrenLocker)
                children = (WObject[])this.Children.Clone();

            for (int i = 0; i < children.Length; i++) children[i].FlagTransformUpdatesRecursive(value, false, doPosition, doRotation, doScale);
        }

        private void FlagTransformUpdates(bool value) => _PositionNeedsUpdate = _RotationNeedsUpdate = _ScaleNeedsUpdate = value;
        private void FlagDirectionUpdates(bool value) => _RightNeedsUpdate = _LeftNeedsUpdate = _UpNeedsUpdate = _DownNeedsUpdate = _ForwardNeedsUpdate = _BackwardNeedsUpdate = value;

        private Vector3D _Right;
        public Vector3D Right
        {
            get
            {
                if (_RightNeedsUpdate)
                {
                    _RightNeedsUpdate = false;
                    _Right = this.Rotation * Vector3D.Right;
                }
                return _Right;
            }
            
            set => Rotation = Quaternion.FromCross(Vector3D.Right, value.Normalized);
        }

        private Vector3D _Left;
        public Vector3D Left
        {
            get
            {
                if (_LeftNeedsUpdate)
                {
                    _LeftNeedsUpdate = false;
                    _Left = this.Rotation * Vector3D.Left;
                }
                return _Left;
            }

            set => this.Right = -value;
        }

        private Vector3D _Up;
        public Vector3D Up
        {
            get
            {
                if (_UpNeedsUpdate)
                {
                    _UpNeedsUpdate = false;
                    _Up = this.Rotation * Vector3D.Up;
                }
                return _Up;
            }

            set => Rotation = Quaternion.FromCross(Vector3D.Up, value.Normalized);
        }

        private Vector3D _Down;
        public Vector3D Down
        {
            get
            {
                if (_DownNeedsUpdate)
                {
                    _DownNeedsUpdate = false;
                    _Down = this.Rotation * Vector3D.Down;
                }
                return _Down;
            }

            set => this.Up = -value;
        }
        
        private Vector3D _Forward;
        public Vector3D Forward
        {
            get
            {
                if (_ForwardNeedsUpdate)
                {
                    _ForwardNeedsUpdate = false;
                    _Forward = this.Rotation * Vector3D.Forward;
                }
                return _Forward;
            }
            
            set => Rotation = Quaternion.FromCross(Vector3D.Forward, value.Normalized);
        }


        private Vector3D _Backward;
        public Vector3D Backward
        {
            get
            {
                if (_BackwardNeedsUpdate)
                {
                    _BackwardNeedsUpdate = false;
                    _Backward = this.Rotation * Vector3D.Backward;
                }
                return _Backward;
            }

            set => this.Forward = -value;
        }


        internal protected virtual Matrix4D TransformMatrix
        {
            get
            {
                Matrix4D scaD =
                    new Matrix4D(this.Scale, true);

                Matrix4D rotD =
                    new Matrix4D(this.Rotation);

                Matrix4D posD =
                    new Matrix4D(this.Position, false);


                return scaD * rotD * posD * Matrix4D.Identity;
            }
        }

        internal protected virtual void TransformMatrixRelativeRef(Vector3D relative, out Matrix4D result)
        {
            Matrix4D scaD =
                     new Matrix4D(this.Scale, true);

            Matrix4D rotD =
                new Matrix4D(this.Rotation);

            Matrix4D posD =
                new Matrix4D(this.Position - relative, false);

            Matrix4D.Mult(in scaD, in rotD, out result);
            Matrix4D.Mult(in result, in posD, out result);
            Matrix4D.Mult(in result, Matrix4D.Identity, out result);
        }

        internal protected virtual void TransformMatrixRef(out Matrix4D result)
        {
            Matrix4D scaD =
                     new Matrix4D(this.Scale, true);

            Matrix4D rotD =
                new Matrix4D(this.Rotation);

            Matrix4D posD =
                new Matrix4D(this.Position, false);

            Matrix4D.Mult(in scaD, in rotD, out result);
            Matrix4D.Mult(in result, in posD, out result);
            Matrix4D.Mult(in result, Matrix4D.Identity, out result);
        }

        public static WObject Find(string name)
        {
            List<WObject> wobjs;
            lock (WobjectsLocker)
                wobjs = WObjects.ToList(); 
            return wobjs.FirstOrDefault(w => w.Name == name);
        }
        
        public static WObject Find(Guid identifier)
        {
            List<WObject> wobjs;
            lock (WobjectsLocker)
                wobjs = WObjects.ToList(); 
            return wobjs.FirstOrDefault(w => w.Identifier == identifier);
        }

        public WObject FindChild(string childName)
        {
            if (this.Deleted) return null;
            
            WObject[] children;
            lock (_ChildrenLocker)
                children = _Children.ToArray();
            return children.FirstOrDefault(c => c.Name == childName);
        }

        public Module AddModule(Type type)
        {
            if(type == null)
            {
                Debug.LogError("Unabled to a null type module to a WObject.");
                return null;
            }
            if(!type.IsSubclassOf(typeof(Module)))
            {
                Debug.LogError("Unabled to add " + type.Name + " to a WObject: it is not a Module.");
                return null;
            }

            Module mod = Activator.CreateInstance(type) as Module;

            if (mod.Undeletable && !this.Undeletable)
            {
                Debug.LogError("Cannot add undeletable module on deletable WObject !");

                Group.GetGroup(0).RemoveModule(mod);
                return null;
            }

            mod.Name = mod.GetType().Name;
            mod.WObject = this;

            this._Modules.Add(mod);

            mod.Creation();

            if (this.Enabled)
            {
                mod.OnEnable();
            }
            else
            {
                mod.OnDisable();
            }

            return mod;
        }
        public T AddModule<T>() where T : Module
        {
            T mod = Activator.CreateInstance<T>();

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
            
            if (this.Enabled)
            {
                mod.OnEnable();
            }
            else
            {
                mod.OnDisable();
            }
            
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


        public override bool Enabled 
        { 
            get
            {
                return ThisAndParentEnabled();
            }
            set
            {
                this._Enabled = value;
            }
        }

        private bool ThisAndParentEnabled()
        {
            return this._Enabled && (!this.Parent || this.Parent.Enabled);
        }

        internal sealed override void ForcedDelete()
        {
            for (int i = 0; i < _Modules.Count; i++)
            {
                _Modules[i].ForcedDelete();
            }

            for (int i = 0; i < _Children.Count; i++)
            {
                _Children[i].ForcedDelete();
            }

            lock(ParentLocker)
                this.Parent = null;
            _Children.Clear();
            _Children = null;

            _Modules.Clear();
            _Modules = null;
            lock (WobjectsLocker)
                WObjects.Remove(this);

            base.Delete();
        }
        public sealed override void Delete()
        {
            if (this.Undeletable)
            {
                Debug.LogWarning("Unable to delete " + this + " : wobject undeletable.");
                return;
            }

            if(this._Modules != null)
                for (int i = 0; i < _Modules.Count; i++)
                    _Modules[i].Delete();

            if(this._Children != null)
                for (int i = 0; i < _Children.Count; i++)
                    _Children[i].Delete();

            lock(ParentLocker)
                this.Parent = null;
            _Children?.Clear();
            _Children = null;

            _Modules?.Clear();
            _Modules = null;
            lock (WobjectsLocker)
                WObjects.Remove(this);

            base.Delete();
        }

        public static void TraceHierarchy()
        {
            Debug.Log("All WObjects: \n" + GetHierachy(null, 0));
        }

        private static string GetHierachy(WObject parent, int recursiveCount)
        {
            string str = parent == null ? "Scene" : parent.Name;
            
            WObject[] children = null;

            if (parent)
            {
                children = parent.Children;
            }
            else
            {
                recursiveCount++;
                children = WObjects.Where(w => w._Parent == null).ToArray();
            }
            
            for (int i = 0; i < children.Length; i++)
            {
                str += "\n";

                
                for (int j = 1; j < recursiveCount; j++)
                {
                    str += "  ";
                }

                if (i == children.Length - 1) str += "└─";
                else str += "├─";
                
                
                
                str += GetHierachy(children[i], recursiveCount + 1);
            }

            return str;
        }
    }
}
