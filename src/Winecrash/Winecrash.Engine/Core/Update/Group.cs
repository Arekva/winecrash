using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public class Group
    {
        internal ManualResetEvent DoneEvent { get; set; } = new ManualResetEvent(false);
        internal ManualResetEvent ResetEvent { get; set; } = new ManualResetEvent(false);

        private int _Order;
        internal int Order
        { 
            get
            {
                return this._Order;
            }

            set
            {
                SetOrder(value);
            }
        }

        private string _Name = "Group";
        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                this._Name = value;
                this.Thread.Name = value + " Thread";
            }
        }

        private int _Layer = 0;
        public int Layer
        {
            get
            {
                return this._Layer;
            }

            set
            {
                SetGroupLayer(this._Order, value);
            }
        }

        internal List<Module> _Modules { get; set; } = new List<Module>(1);
        internal readonly static object moduleLocker = new object();
        internal readonly static object groupLocker = new object();
        internal static List<Group> _Groups { get; set; } = new List<Group>(1);
        internal int GroupCount
        {
            get
            {
                return _Groups == null ? 0 : _Groups.Count;
            }
        }

        internal Thread Thread { get; private set; }

        internal bool Deleted { get; private set; }

        private Group(string name, int order, int layer, IEnumerable<Module> modules)
        {
            if (name == null) this._Name = "Group #" + order;
            else this._Name = name;

            lock (moduleLocker)
                _Modules = modules?.ToList();

            this._Order = order;

            lock(groupLocker)
                _Groups.Add(this);

            Engine.Layer.CreateOrGetLayer(layer, null, new[] { this });

            SortByOrder();

            Thread = new Thread(Update)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true,
                Name = this._Name + " Thread"
            };

            Thread.Start();
        }

        internal static UpdateTypes UpdateType = UpdateTypes.PreUpdate;

        internal void FixedUpdate()
        {
            Module[] modules = null;

            lock (moduleLocker)
                modules = this._Modules?.ToArray();

            if (modules != null)
            {
                for (int i = 0; i < modules.Length; i++)
                {
                    try
                    {
                        modules[i]?.FixedUpdate();
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        internal void LateFixedUpdate()
        {
            if(this._Modules != null)
            {
                List<Module> modules = null;

                lock(moduleLocker)
                    modules = this._Modules.ToList();

                foreach (Module mod in modules)
                {
                    try
                    {
                        mod?.LateFixedUpdate();
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
        private void Update()
        {
            while (true)
            {
                UpdateTypes ut = UpdateType;
                this.ResetEvent.WaitOne(); //wait for reset event
                this.ResetEvent.Reset(); //set reset event to false

                List<Module> modules = null;

                lock(moduleLocker)
                    modules = _Modules?.ToList();

                if (modules != null)
                {
                    foreach (Module mod in modules)
                    {
                        if (mod == null || !mod.Enabled || mod.Deleted) continue;
                        
                        try
                        {
                            switch (ut)
                            {
                                case UpdateTypes.PreUpdate:
                                    {
                                        if (mod.StartDone == false)
                                        {
                                            mod.StartDone = true;
                                            if (mod.RunAsync)
                                            {
                                                Task.Run(mod.Start);
                                            }
                                            else
                                            {
                                                mod.Start();
                                            }
                                        }
            
                                        mod.PreUpdate();
                                    }
                                    break;

                                case UpdateTypes.Update:
                                    {
                                        mod.Update();
                                    }
                                    break;

                                case UpdateTypes.LateUpdate:
                                    {
                                        mod.LateUpdate();
                                    }
                                    break;
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }

                this.DoneEvent.Set(); //says the thread is done.      
            }  
        }

        internal static Group CreateOrGetGroup(int order, string name = null, IEnumerable<Module> modules = null)
        {
            Group group = GetGroup(order);

            if (group == null) // no layer, can add one.
            {
                group = new Group(name, order, 0, modules);
            }

            else
            {
                if(modules != null)
                {
                    lock (moduleLocker)
                        group._Modules?.AddRange(modules);
                }
            }

            return group;
        }

        internal void RemoveModule(Module module)
        {
            if (this._Modules == null) return;

            lock (moduleLocker)
                this._Modules.Remove(module);

            if(this._Order != 0 && this._Modules.Count == 0) //remove if none
            {
                lock (moduleLocker)
                    this._Modules = null;

                lock (groupLocker)
                    _Groups.Remove(this);

                foreach(Layer layer in Engine.Layer._Layers)
                {
                    if(layer._Groups.Contains(this))
                    {
                        layer._Groups.Remove(this);

                        if(layer._Groups.Count == 0) //remove layer
                        {
                            Engine.Layer.RemoveLayer(layer.Order);

                        }

                        break;
                    }
                }

                this.Thread.Abort();
                this.Deleted = true;

                this.DoneEvent.Set();
            }
        }

        private void RemoveGroup(int order)
        {
            if (order == 0)
            {
                Debug.LogWarning($"Group.cs: unable to remove group 0.");

                return;
            }
            Group group = GetGroup(order);

            if (group != null)
            {
                group.Thread.Abort();
                group.Thread = null;

                group.Thread.Abort();

                lock (groupLocker)
                    _Groups.Remove(group);

                CreateOrGetGroup(0, null, group._Modules);
            }
            else
            {
                Debug.LogWarning($"Layer.cs: tried to remove layer {order}, but it is not existing.");
            }
        }

        private void SetOrder(int newOrder)
        {
            if (this._Order == 0) return;

            Group group = GetGroup(newOrder);

            if (group != null)
            {
                MergeGroups(this, group);
            }

            this._Order = newOrder;
            SortByOrder();
        }

        private static Group MergeGroups(Group first, Group second)
        {
            if (first == null || second == null)
            {
                Debug.LogWarning($"Group.cs: impossible to merge group {first} and {second}: one of them is null.");

                return null;
            }

            lock (moduleLocker)
                first._Modules.AddRange(second._Modules);

            lock (moduleLocker)
                second._Modules = null;

            second.Thread.Abort();
            second.Thread = null;

            lock(groupLocker)
                _Groups.Remove(second);

            return first;
        }

        public static Group GetGroup(int order)
        {
            List<Group> groups = null;
            //lock(moduleLocker)
            lock (groupLocker)
                groups = _Groups.ToList();


            return groups.FirstOrDefault(g => g.Order == order);
        }

        public static Group GetGroup(string name)
        {
            List<Group> groups = null;
            lock (groupLocker)
                groups = _Groups.ToList();

            return groups.FirstOrDefault(g => g.Name == name);
        }

        private static void SortByOrder()
        {
            lock(groupLocker)
                _Groups = _Groups.OrderBy(g => g.Order).ToList();
        }

        internal void SortModules()
        {
            lock (moduleLocker)
                this._Modules = _Modules.OrderBy(m => m.ExecutionOrder).ToList();
        }

        internal static void SetGroupLayer(int group, int newLayer)
        {
            if (group == 0) return;

            Group correspondingGroup = Group.GetGroup(group);

            if(correspondingGroup != null)
            {
                Layer[] layers = Engine.Layer._Layers.ToArray();

                for (int i = 0; i < layers.Length; i++)
                {
                    //find the current layer of the group
                    if (layers[i]._Groups.Contains(correspondingGroup))
                    {
                        layers[i]._Groups.Remove(correspondingGroup);
                        Engine.Layer.CreateOrGetLayer(newLayer, null, new[] { correspondingGroup });

                        if (layers[i]._Groups.Count == 0) //remove layer
                        {
                            Engine.Layer.RemoveLayer(layers[i].Order);
                        }


                        break;
                    }
                }
            }
        }
    }
}
