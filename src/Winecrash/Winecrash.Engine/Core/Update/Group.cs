using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    internal class Group
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

        internal List<Module> _Modules = new List<Module>(1);

        internal static List<Group> _Groups = new List<Group>(1);
        public int GroupCount
        {
            get
            {
                return _Groups.Count;
            }
        }
        
        public Thread Thread { get; private set; }

        private bool DoUpdate = false;
        public bool UpdateDone { get; private set; } = false;

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

        public void TriggerUpdate()
        {
            DoUpdate = true;
        }

        private void Update()
        {
            while(true)
            {
                while (!DoUpdate)
                {
                    Thread.Sleep(1);
                }
                UpdateDone = false;
                DoUpdate = false;

                foreach (Module module in this._Modules)
                {
                    module.Update();
                }

                UpdateDone = true;
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

                List<Module> modules = group._Modules;
                _Groups.Remove(group);
                CreateOrGetGroup(0, null, modules);
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

        private static Group GetGroup(int order)
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
    }
}
