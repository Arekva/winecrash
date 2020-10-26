using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace WEngine
{
    internal class Layer
    {
        private int _Order;
        public int Order
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

        internal List<Group> _Groups { get; set; } = new List<Group>(1);
        internal object GroupsLocker { get; set; } = new object();

        internal bool Deleted { get; set; } = false;

        public static Thread FixedThread;

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

            FixedThread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                while (true)
                {
                    sw.Start();

                    Layer[] layers = _Layers.ToArray();

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
                            if (groups[j].Deleted) continue;
                            groups[j].LateFixedUpdate();
                        }
                    }

                    sw.Stop();

                    double waitTime = (Physics.FixedRate * Time.FixedTimeScale) - sw.Elapsed.TotalSeconds;

                    if (waitTime > 0.0D)
                    {
                        Thread.Sleep((int)(waitTime * 1000));
                    }


                    Time.FixedDeltaTime = waitTime;//Physics.FixedRate * Time.FixedTimeScale;

                   /* else
                    {
                        Time.FixedDeltaTime = sw.Elapsed.TotalSeconds;
                    }*/

                    sw.Reset();
                }
            })
            {
                Priority = ThreadPriority.Highest
            };

            if (Engine.DoGUI)
                Graphics.Window.OnLoaded += FixedThread.Start;
            else 
                FixedThread.Start();
        }

        private Layer(string name, int order, IEnumerable<Group> groups)
        {
            if (name == null) this.Name = "Layer #" + order;
            else this.Name = name;

            this._Order = order;

            if(groups != null)
                lock(GroupsLocker)
                    this._Groups = groups.ToList();

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
                    layer._Groups.AddRange(groups);
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
                List<Group> groups = layer._Groups;
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
        public static Layer GetLayer(int order)
        {
            return _Layers.FindLast(l => l.Order == order);
        }
        /// <summary>
        /// Gets a layer by its name. Null if none.
        /// </summary>
        /// <param name="name">The name of the layer</param>
        /// <returns></returns>
        public static Layer GetLayer(string name)
        {
            return _Layers.FindLast(l => l.Name == name);
        }

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
                    first._Groups.AddRange(second._Groups);
                
                second._Groups = null;
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
            Layer[] layers = _Layers.ToArray();

            //preupdate
            for (int i = 0; i < layers.Length; i++)
            {
                Group.UpdateType = UpdateTypes.PreUpdate;
                Layer layer = layers[i];

                if (layer.Deleted) continue;

                List<ManualResetEvent> doneEvents = new List<ManualResetEvent>(layer._Groups.Count);

                for (int j = 0; j < layer._Groups.Count; j++)
                {
                    if (layer._Groups[j].Deleted) continue;

                    layer._Groups[j].DoneEvent.Reset();
                    doneEvents.Add(layer._Groups[j].DoneEvent);
                    layer._Groups[j].ResetEvent.Set(); //unlock thread
                }

                WaitHandle.WaitAll(doneEvents.ToArray()); //wait for all the threads of the group
            }

            //update
            for (int i = 0; i < layers.Length; i++)
            {
                Group.UpdateType = UpdateTypes.Update;
                Layer layer = layers[i];

                if (layer.Deleted) continue;

                List<ManualResetEvent> doneEvents = new List<ManualResetEvent>(layer._Groups.Count);

                for (int j = 0; j < layer._Groups.Count; j++)
                {
                    if (layer._Groups[j].Deleted) continue;

                    layer._Groups[j].DoneEvent.Reset();
                    doneEvents.Add(layer._Groups[j].DoneEvent);
                    layer._Groups[j].ResetEvent.Set(); //unlock thread
                }

                WaitHandle.WaitAll(doneEvents.ToArray()); //wait for all the threads of the group
            }

            //late update
            for (int i = 0; i < layers.Length; i++)
            {
                Group.UpdateType = UpdateTypes.LateUpdate;
                Layer layer = layers[i];

                if (layer.Deleted) continue;

                Group[] groups = null;
                lock (layer.GroupsLocker)
                    groups = layer._Groups.ToArray();

                List<ManualResetEvent> doneEvents = new List<ManualResetEvent>(groups.Length);

                for (int j = 0; j < groups.Length; j++)
                {
                    if (groups[j].Deleted) continue;

                    groups[j].DoneEvent.Reset();
                    doneEvents.Add(groups[j].DoneEvent);
                    groups[j].ResetEvent.Set(); //unlock thread
                }

                WaitHandle.WaitAll(doneEvents.ToArray()); //wait for all the threads of the group
            }
        }

        public static string GetTrace()
        {
            string txt = "== Update Execution Trace ==\n\n";

            foreach(Layer layer in _Layers)
            {
                txt += layer.Name + "\n";

                foreach(Group group in layer._Groups)
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
