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

        internal static List<Group> _Groups { get; set; } = new List<Group>(1);
        internal int GroupCount
        {
            get
            {
                return _Groups.Count;
            }
        }

        internal Thread Thread { get; private set; }

        internal bool Deleted { get; private set; }

        private Group(string name, int order, int layer, IEnumerable<Module> modules)
        {
            if (name == null) this._Name = "Group #" + order;
            else this._Name = name;

            if (modules != null)
                _Modules = modules.ToList();

            this._Order = order;

            _Groups.Add(this);

            Engine.Layer.CreateOrGetLayer(layer, null, new[] { this });

            SortByOrder();

            Thread = new Thread(Update)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = false,
                Name = this._Name + " Thread"
            };

            Thread.Start();
        }

        internal static UpdateTypes UpdateType = UpdateTypes.PreUpdate;

        internal void FixedUpdate()
        {
            Module[] modules = this._Modules.ToArray();

            if(modules != null)
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i]?.FixedUpdate();
            }
        }

        internal void LateFixedUpdate()
        {
            if (this._Modules != null)
            {
                Module[] modules = this._Modules.ToArray();

                for (int i = 0; i < modules.Length; i++)
                {
                    modules[i]?.LateFixedUpdate();
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

                Module[] modules = this._Modules.ToArray();
                
                for (int i = 0; i < modules.Length; i++)
                {
                    if(modules[i] != null)
                    lock (modules[i])
                    {
                        if (modules[i].Deleted) continue;

                        switch(ut)
                            {
                                case UpdateTypes.PreUpdate:
                                    {
                                        if (modules[i].StartDone == false)
                                        {
                                            modules[i].StartDone = true;
                                            if (modules[i].RunAsync)
                                            {
                                                Task.Run(modules[i].Start);
                                            }
                                            else
                                            {
                                                modules[i].Start();
                                            }
                                        }

                                        modules[i].PreUpdate();
                                    }
                                    break;

                                case UpdateTypes.Update:
                                    {
                                         modules[i].Update();
                                    }
                                    break;

                                case UpdateTypes.LateUpdate:
                                    {
                                        modules[i].LateUpdate();
                                    }
                                    break;
                            }

                        //modules[i].Update();
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
                if (modules != null)
                {
                    group._Modules.AddRange(modules);
                }
            }

            return group;
        }

        internal void RemoveModule(Module module)
        {
            this._Modules.Remove(module);

            if(this._Order != 0 && this._Modules.Count == 0) //remove if none
            {
                this._Modules = null;
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

            first._Modules.AddRange(second._Modules);

            second._Modules = null;
            second.Thread.Abort();
            second.Thread = null;

            _Groups.Remove(second);

            return first;
        }

        public static Group GetGroup(int order)
        {
            return _Groups.FindLast(g => g.Order == order);
        }

        public static Group GetGroup(string name)
        {
            return _Groups.FindLast(g => g.Name == name);
        }

        private static void SortByOrder()
        {
            _Groups = _Groups.OrderBy(g => g.Order).ToList();
        }

        internal void SortModules()
        {
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
