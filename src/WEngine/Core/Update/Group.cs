﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WEngine
{
    /// <summary>
    /// A group is an ensemble of scripts executing one by one. Groups within the same <see cref="Layer"/> run in parallel. <see cref="Layer"/>s run one by one.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The update loop done event.
        /// </summary>
        internal ManualResetEvent DoneEvent { get; set; } = new ManualResetEvent(false);
        /// <summary>
        /// The update loop reset event.
        /// </summary>
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
        //internal readonly static object groupLocker = new object();
        internal static List<Group> _Groups { get; set; } = new List<Group>(1);
        internal static object GroupsLocker { get; set; } = new object();
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

            if (_Modules == null) 
                _Modules = new List<Module>();

            this._Order = order;

            lock(GroupsLocker)
                _Groups.Add(this);

            WEngine.Layer.CreateOrGetLayer(layer, null, new[] { this });

            SortByOrder();

            Thread = new Thread(Update)
            {
                Priority = ThreadPriority.AboveNormal,
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
                modules = this._Modules.OrderBy(m => m?.FixedExecutionOrder ?? int.MinValue).ToArray();

            if (modules != null)
            {
                for (int i = 0; i < modules.Length; i++)
                {
                    if (modules[i] == null || !modules[i].Enabled || modules[i].Deleted) continue;
                    
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
                    modules = this._Modules.OrderBy(m => m?.FixedExecutionOrder ?? int.MinValue).ToList();

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
        internal void PreFixedUpdate()
        {
            if(this._Modules != null)
            {
                List<Module> modules = null;

                lock(moduleLocker)
                    modules = this._Modules.OrderBy(m => m?.FixedExecutionOrder ?? int.MinValue).ToList();

                foreach (Module mod in modules)
                {
                    try
                    {
                        mod?.PreFixedUpdate();
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
            while (!this.Deleted)
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

                lock (GroupsLocker)
                    _Groups.Remove(this);

                foreach(Layer layer in WEngine.Layer._Layers)
                {
                    lock (layer.GroupsLocker)
                    {
                        if (layer._Groups.Contains(this))
                        {
                            layer._Groups.Remove(this);

                            if (layer._Groups.Count == 0) //remove layer
                            {
                                WEngine.Layer.RemoveLayer(layer.Order);

                            }

                            break;
                        }
                    }
                }
                
                this.Deleted = true;
                this.Thread = null;
                
                this.DoneEvent.Set();
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

            lock(GroupsLocker)
                _Groups.Remove(second);
            
            second.Deleted = true;
            second.Thread = null;

            return first;
        }

        public static Group GetGroup(int order)
        {
            List<Group> groups = null;
            //lock(moduleLocker)
            lock (GroupsLocker)
                groups = _Groups.ToList();


            return groups.FirstOrDefault(g => g.Order == order);
        }

        public static Group GetGroup(string name)
        {
            List<Group> groups = null;
            lock (GroupsLocker)
                groups = _Groups.ToList();

            return groups.FirstOrDefault(g => g.Name == name);
        }

        private static void SortByOrder()
        {
            lock(GroupsLocker)
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
                Layer[] layers = WEngine.Layer._Layers.ToArray();

                for (int i = 0; i < layers.Length; i++)
                {
                    //find the current layer of the group
                    lock (layers[i].GroupsLocker)
                    {
                        if (layers[i]._Groups.Contains(correspondingGroup))
                        {
                            layers[i]._Groups.Remove(correspondingGroup);
                            WEngine.Layer.CreateOrGetLayer(newLayer, null, new[] {correspondingGroup});

                            if (layers[i]._Groups.Count == 0) //remove layer
                            {
                                WEngine.Layer.RemoveLayer(layers[i].Order);
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
