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
        public ManualResetEvent DoneEvent { get; set; } = new ManualResetEvent(false);
        public ManualResetEvent ResetEvent { get; set; } = new ManualResetEvent(false);

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

        internal List<Module> _Modules { get; set; } = new List<Module>(1);

        internal static List<Group> _Groups { get; set; } = new List<Group>(1);
        public int GroupCount
        {
            get
            {
                return _Groups.Count;
            }
        }
        
        public Thread Thread { get; private set; }

        private Group(string name, int order, IEnumerable<Module> modules)
        {
            if (name == null) this._Name = "Group #" + GroupCount;
            else this._Name = name;

            if (modules != null)
                _Modules = modules.ToList();

            this._Order = order;

            _Groups.Add(this);


            SortByOrder();

            Thread = new Thread(Update)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = false,
                Name = this._Name + " Thread"
            };

            Thread.Start();
        }

        private void Update()
        {
            
            while (true)
            {
                this.ResetEvent.WaitOne(); //wait for reset event
                this.ResetEvent.Reset(); //set reset event to false

                Module[] modules = this._Modules.ToArray();
                
                for (int i = 0; i < modules.Length; i++)
                {
                    if (modules[i].Deleted) continue;

                    if (!modules[i].StartDone)
                    {
                        modules[i].StartDone = true;
                        modules[i].Start();
                    }

                    modules[i].Update();
                }

                this.DoneEvent.Set(); //says the thread is done.      
            }  
        }

        public static Group CreateOrGetGroup(int order, string name = null, IEnumerable<Module> modules = null)
        {
            Group group = GetGroup(order);

            if (group == null) // no layer, can add one.
            {
                group = new Group(name, order, modules);
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

        public void RemoveModule(Module module)
        {
            this._Modules.Remove(module);
        }

        public void RemoveGroup(int order)
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

        private static Group GetGroup(string name)
        {
            return _Groups.FindLast(g => g.Name == name);
        }

        private static void SortByOrder()
        {
            _Groups = _Groups.OrderBy(g => g.Order).ToList();
        }

        public void SortModules()
        {
            this._Modules = _Modules.OrderBy(m => m.ExecutionOrder).ToList();
        }

        public static void SetGroupLayer(int group, int newLayer)
        {
            Group correspondingGroup = Group.GetGroup(group);

            if(correspondingGroup != null)
            {
                foreach(Layer layer in Layer._Layers)
                {
                    //find the current layer of the group
                    if (layer._Groups.Contains(correspondingGroup))
                    {
                        layer._Groups.Remove(correspondingGroup);
                        Layer.CreateOrGetLayer(newLayer, null, new[] { correspondingGroup });
                        break;
                    }
                }
            }
        }
    }
}
