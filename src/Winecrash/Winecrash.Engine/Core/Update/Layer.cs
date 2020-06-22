﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winecrash.Engine
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
        public static int LayerCount
        {
            get
            {
                return _Layers.Count;
            }
        }

        internal List<Group> _Groups = new List<Group>(1);

        [Initializer]
        private static void Initialize()
        {
            CreateOrGetLayer(0, "Default Layer", new[] { Group.CreateOrGetGroup(0, "Default Group", null) });

            Viewport.Update += new UpdateEventHandler(Update);
        }

        private Layer(string name, int order, IEnumerable<Group> groups)
        {
            if (name == null) this.Name = "Layer #" + LayerCount;
            else this.Name = name;

            this._Order = order;

            if(groups != null)
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
            _Layers = _Layers.OrderBy(l => l.Order).ToList();
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

            first._Groups.AddRange(second._Groups);
            second._Groups = null;
            _Layers.Remove(second);

            return first;
        }

        public static void Update(UpdateEventArgs args)
        {
            foreach (Layer layer in _Layers)
            {
                ManualResetEvent[] doneEvents = new ManualResetEvent[layer._Groups.Count];

                for (int i = 0; i < layer._Groups.Count; i++)
                {
                    layer._Groups[i].DoneEvent.Reset();
                    doneEvents[i] = layer._Groups[i].DoneEvent;
                    layer._Groups[i].ResetEvent.Set(); //unlock thread
                }

                WaitHandle.WaitAll(doneEvents); //wait for all the threads of the group
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
                    txt += "-" + group.Name + "\n";

                    foreach(Module module in group._Modules)
                    {
                        txt += "--" + module.Name + "\n";
                    }
                }
            }

            return txt;
        }
    }
}