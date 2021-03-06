﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WEngine
{
    internal class Layer
    {
        private int _Order;
        public int Order
        {
            get => this._Order;
            

            set => SetOrder(value);
            
        }

        public string Name { get; set; } = "Layer";

        internal static List<Layer> _Layers = new List<Layer>(1);
        internal static object LayerLocker = new object();

        public static int LayerCount
        {
            get
            {
                return _Layers.Count;
            }
        }

        private volatile List<Group> _groups = new List<Group>(1);

        internal List<Group> Groups
        {
            get
            {
                lock (GroupsLocker) return _groups;
            }
            set
            {
                lock (GroupsLocker) _groups = value;
            }
        }

        internal void AddGroup(Group group)
        {
            lock (GroupsLocker) _groups.Add(group);
        }
        internal void AddRangeGroup(IEnumerable<Group> groups)
        {
            lock (GroupsLocker) _groups.AddRange(groups);
        }
        internal void RemoveGroup(Group group)
        {
            lock (GroupsLocker) _groups.Remove(group);
        }
        internal bool ContainsGroup(Group group) => this.Groups.ToArray().Contains(group);
        
        internal int CountGroup() => this.Groups.ToArray().Length;

        internal object GroupsLocker { get; set; } = new object();
        
        internal static ManualResetEvent PhysicsThreadLocker { get; set; } = new ManualResetEvent(false);
        internal static ManualResetEvent RenderThreadLocker { get; set; } = new ManualResetEvent(true);

        internal bool Deleted { get; set; } = false;

        public static Thread PhysicsThread;

        //[Initializer]
        internal static void Initialize()
        {
            Layer.CreateOrGetLayer(0, "Default Layer", null);
            Group.CreateOrGetGroup(0, "Default Group", null);
            
            if (Engine.DoGUI)
            {
                Graphics.Window.OnUpdate += new UpdateEventHandler(Update);
                Graphics.Window.OnRender += new UpdateEventHandler(Render);
            }
          
            /*FixedThread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                while (true)
                {
                    PhysicsThreadLocker.WaitOne();
                    RenderThreadLocker.Reset();
                    //Task.Delay(1).Wait();
                    //Debug.Log("PHY START");
                    sw.Start();

                    Layer[] layers = _Layers.ToArray();
                    
                    
                    for (int i = 0; i < layers.Length; i++)
                    {
                        Layer layer = layers[i];

                        if (layer.Deleted) continue;

                        Group[] groups;
                        lock (layer.GroupsLocker) 
                            groups = layer._Groups.ToArray();


                        if (groups == null) continue;

                        for (int j = 0; j < groups.Length; j++)
                        {
                            if (groups[j] == null || groups[j] != null && groups[j].Deleted) continue;
                            groups[j].PreFixedUpdate();
                        }
                    }

                    //fixedupdate
                    for (int i = 0; i < layers.Length; i++)
                    {
                        Layer layer = layers[i];

                        if (layer.Deleted) continue;

                        Group[] groups;
                        lock(layer.GroupsLocker)
                            groups = layer._Groups.ToArray();

                        if (groups == null) continue;

                        for (int j = 0; j < groups.Length; j++)
                        {
                            if (groups[j] == null || groups[j].Deleted) continue;
                            groups[j].FixedUpdate();
                        }
                    }

                    for (int i = 0; i < layers.Length; i++)
                    {
                        Layer layer = layers[i];

                        if (layer.Deleted) continue;

                        Group[] groups;
                        lock (layer.GroupsLocker) 
                            groups = layer._Groups.ToArray();


                        if (groups == null) continue;

                        for (int j = 0; j < groups.Length; j++)
                        {
                            if (groups[j] == null || groups[j] != null && groups[j].Deleted) continue;
                            groups[j].LateFixedUpdate();
                        }
                    }
                    
                    

                    sw.Stop();
                    //Debug.Log("PHY STOP");
                    RenderThreadLocker.Set();
                    double waitTime = Physics.FixedRate - sw.Elapsed.TotalSeconds;
                    sw.Start();
                    if (waitTime > 0.0D)
                    {
                        Thread.Sleep((int)(waitTime * 1000));
                    }

                    sw.Stop();
                    Time.FixedDeltaTime = sw.Elapsed.TotalSeconds * Time.FixedTimeScale;
                    sw.Reset();
                }
            })
            {
                Priority = ThreadPriority.Highest,
                Name = "Fixed"
            };

            if (Engine.DoGUI)
                Graphics.Window.OnLoaded += FixedThread.Start;
            else 
                FixedThread.Start();*/

            if (Engine.DoGUI) Graphics.Window.OnLoaded += () => StartPhysicsThread();
            else StartPhysicsThread();
        }

        private Layer(string name, int order, IEnumerable<Group> groups)
        {
            if (name == null) this.Name = "Layer #" + order;
            else this.Name = name;

            this._Order = order;

            if(groups != null)
                lock(GroupsLocker)
                    this.Groups = groups.ToList();

            _Layers.Add(this);

            SortByOrder();
        }

        /// <summary>
        /// Create or get a layer. If the layer is already existing, adds the group.
        /// </summary>
        /// <param name="order">The order of the layer.</param>
        /// <param name="groups">The groups that has to be added to the layer</param>
        public static Layer CreateOrGetLayer(int order, string name = null, IEnumerable<Group> groups = null)
        {
            Layer layer = GetLayer(order);

            if (layer == null) // no layer, can add one.
            {
                layer = new Layer(name, order, groups);
            }

            else
            {
                if(groups != null)
                {
                    layer.AddRangeGroup(groups);
                }
            }

            return layer;
        }

        /// <summary>
        /// Remove layers and move the groups into the default layer
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static void RemoveLayer(int order)
        {
            if(order == 0)
            {
                Debug.LogWarning($"Layer.cs: unable to remove layer 0.");

                return;
            }
            Layer layer = GetLayer(order);

            if(layer != null)
            {
                List<Group> groups = layer.Groups;
                _Layers.Remove(layer);
                layer.Deleted = true;
                CreateOrGetLayer(0, null, groups);
            }
            else
            {
                Debug.LogWarning($"Layer.cs: tried to remove layer {order}, but it is not existing.");
            }
        }

        /// <summary>
        /// Sets the order of the layer. Merge layers if the order exists.
        /// </summary>
        /// <param name="newOrder"></param>
        /// <returns></returns>
        private void SetOrder(int newOrder)
        {
            Layer layer = GetLayer(newOrder);

            if (layer != null) //if layer corresponding merge current and old (keep current)
            {
                MergeLayers(this, layer);
            }

            this._Order = newOrder;
            SortByOrder();
        }

        /// <summary>
        /// Gets a layer by its order. Null if none.
        /// </summary>
        /// <param name="order">The order of the layer</param>
        /// <returns></returns>
        public static Layer GetLayer(int order) => _Layers.FindLast(l => l.Order == order);
        
        /// <summary>
        /// Gets a layer by its name. Null if none.
        /// </summary>
        /// <param name="name">The name of the layer</param>
        /// <returns></returns>
        public static Layer GetLayer(string name) => _Layers.FindLast(l => l.Name == name);
        

        /// <summary>
        /// Sorts layers by the orders.
        /// </summary>
        private static void SortByOrder()
        {
            if(_Layers != null)
            {
                lock(LayerLocker)
                {
                    _Layers = _Layers.Where(l => l != null).OrderBy(l => l.Order).ToList();
                }
            }
        }

        /// <summary>
        /// Merge two layers together, return the first one.
        /// </summary>
        /// <param name="first">The first layer</param>
        /// <param name="second">The second layer</param>
        /// <returns></returns>
        private static Layer MergeLayers(Layer first, Layer second)
        {
            if(first == null || second == null)
            {
                Debug.LogWarning($"Layer.cs: impossible to merge layer {first} and {second}: one of them is null.");

                return null;
            }

            
            lock (second.GroupsLocker)
            {
                lock(first.GroupsLocker)
                    first.AddRangeGroup(second.Groups);
                
                second.Groups = null;
            }

            _Layers.Remove(second);

            return first;
        }

        public static void Render(UpdateEventArgs args)
        {
            Camera[] cameras = Camera.Cameras.OrderBy(cam => cam.Depth).ToArray();

            for (int i = 0; i < cameras.Length; i++)
            {
                //todo: threaded OnRender - note: nope lol (well, when I'll finally make the game run onto Vulkan).
                Module[] modules = cameras[i].WObject._Modules.ToArray();

                for (int j = 0; j < modules.Length; j++)
                {
                    if (modules[j] != cameras[i])
                    {
                        modules[j].OnRender();
                    }
                }

                cameras[i].OnRender(); // cameras are rendered one by one because of opengl.
            }
        }

        public static void Update(UpdateEventArgs args)
        {
            Layer[] layers = null;
            lock(LayerLocker)
                layers = _Layers.ToArray();

            for (int i = 0; i < layers.Length; i++)
            {
                UpdateGroups(layers[i], UpdateTypes.EarlyUpdate);
                UpdateGroups(layers[i], UpdateTypes.Update);
                UpdateGroups(layers[i], UpdateTypes.LateUpdate);
            }
        }

        public static Thread StartPhysicsThread()
        {
            Thread physicsThread = PhysicsThread = new Thread(PhysicsLoop)
            {
                Name = "Physics",
                IsBackground = false,
                Priority = ThreadPriority.Highest
            };
            physicsThread.Start();
            return physicsThread;
        }

        public static ManualResetEvent UpdateRenderEvent = new ManualResetEvent(false);

        public static ManualResetEvent PhysicsEvent = new ManualResetEvent(true);
        //public static ManualResetEvent PhysicsThreadResetEvent = new ManualResetEvent(true);
        private static void PhysicsLoop()
        {
            Stopwatch frameLengthWatch = new Stopwatch();
            Stopwatch frameBetweenWatch = new Stopwatch();
            
            double waitTime = 0.0;
            while (Engine.Running)
            {
                
                UpdateRenderEvent.WaitOne(); // wait for update/render to be free
                PhysicsEvent.Reset(); // set physics thread as busy
                if(!Engine.Running) break;

                frameBetweenWatch.Stop();
                frameLengthWatch.Restart();
                PhysicsUpdate();
                frameLengthWatch.Stop();
                PhysicsEvent.Set();  // set physics thread as free

                double timeLoss = 0.0D;//frameBetweenWatch.Elapsed.TotalSeconds - waitTime;
                //Debug.Log("physics loss: " + timeLoss);
                waitTime = Math.Max(0.0D, Time.PhysicsRateInverted - frameLengthWatch.Elapsed.TotalSeconds - timeLoss);
                frameBetweenWatch.Restart();
                Thread.Sleep((int)(waitTime * 1000.0D));
            }
        }
        
        public static void PhysicsUpdate()
        {
            Layer[] layers = null;
            lock(LayerLocker)
                layers = _Layers.ToArray();

            for (int i = 0; i < layers.Length; i++)
            {
                PhysicsUpdateGroups(layers[i], UpdateTypes.EarlyPhysics);
                PhysicsUpdateGroups(layers[i], UpdateTypes.Physics);
                PhysicsUpdateGroups(layers[i], UpdateTypes.LatePhysics);
            }
        }
        
        private static void PhysicsUpdateGroups(Layer layer, UpdateTypes updateType)
        {
            Group.PhysicsUpdateType = updateType;
            
            if (layer == null || layer.Deleted) return;

            Group[] groups = null;
            lock (layer.GroupsLocker)
                groups = layer.Groups.ToArray();

            int n = groups.Length;

            List<ManualResetEvent> doneEvents = new List<ManualResetEvent>(n);

            for (int i = 0; i < n; i++)
            {
                Group group = groups[i];

                if (group == null || group.Deleted) continue;

                group.DoneEventPhysics.Reset();
                doneEvents.Add(group.DoneEventPhysics);
                group.ResetEventPhysics.Set(); //unlock thread
            }
            //wait for all the threads of the group
            WaitHandle.WaitAll(doneEvents.ToArray());
        }
        
        private static void UpdateGroups(Layer layer, UpdateTypes updateType)
        {
            Group.UpdateType = updateType;
            
            if (layer == null || layer.Deleted) return;

            Group[] groups = null;
            lock (layer.GroupsLocker)
                groups = layer.Groups.ToArray();

            int n = groups.Length;

            List<ManualResetEvent> doneEvents = new List<ManualResetEvent>(n);

            for (int i = 0; i < n; i++)
            {
                Group group = groups[i];

                if (group == null || group.Deleted) continue;

                group.DoneEvent.Reset();
                doneEvents.Add(group.DoneEvent);
                group.ResetEvent.Set(); //unlock thread
            }

            //wait for all the threads of the group
            WaitHandle.WaitAll(doneEvents.ToArray());
        }

        public static string GetTrace()
        {
            string txt = "== Update Execution Trace ==\n\n";

            foreach(Layer layer in _Layers)
            {
                txt += layer.Name + "\n";

                foreach(Group group in layer.Groups)
                {
                    txt += "--" + group.Name + "\n";

                    foreach(Module module in group._Modules)
                    {
                        txt += "----" + module.Name + "[" + module.ExecutionOrder + "]\n";
                    }
                }
            }

            return txt;
        }
    }
}
