using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WEngine
{
    /// <summary>
    /// The callback delegate used into the <see cref="Initializer"/>.
    /// </summary>
    internal delegate void InitializerCallback();

    /// <summary>
    /// <see cref="Engine"/> initalizer: add this attribute to any static non public method and the engine will execute it at start.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class Initializer : Attribute, IComparable, IComparable<Initializer>, IEquatable<Initializer>
    {
        /// <summary>
        /// The execution order of the script (higher > later it will be executed).
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// The actual method linked to the initializer.
        /// </summary>
        private InitializerCallback Method;

        /// <summary>
        /// Is the engine initialized.
        /// </summary>
        private static bool Initialized = false;

        /// <summary>
        /// Create an initializer method for the <see cref="WEngine"/> to execute at start. Must be private and static.
        /// </summary>
        public Initializer()
        {
            this.Order = 0;
        }
        /// <summary>
        /// Create an initializer method for the <see cref="WEngine"/> to execute at start. Must be private and static.
        /// </summary>
        /// <param name="order">At what order does the script execute? (Higher -> later)</param>
        public Initializer(int order)
        {
            this.Order = order;
        }

        /// <summary>
        /// Trigger all initializer scripts. Only can be done once.
        /// </summary>
        public static void InitializeEngine()
        {
            if (Initialized) return;
            Initialized = true;

            MethodInfo[] methods = 
                Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttributes(typeof(Initializer), false).Length > 0)
                .ToArray();

            List<Initializer> initializers = new List<Initializer>(methods.Length);

            for (int i = 0; i < methods.Length; i++)
            {
                Initializer init = (Initializer)methods[i].GetCustomAttribute(typeof(Initializer), false);

                try
                {
                    InitializerCallback callback = (InitializerCallback)Delegate.CreateDelegate(typeof(InitializerCallback), methods[i]);
                    init.Method = callback;

                    initializers.Add(init);
                }

                catch(Exception e)
                {
                    Debug.LogError("Engine Initializer: error when tring to init " + init.Method + ": " + e);
                }
            }

            initializers.Sort();

            foreach(Initializer init in initializers)
            {
                init.Method.Invoke();
            }
        }

        #region Interfaces Implementations
        public int CompareTo(object obj)
        {
            return CompareTo(obj as Initializer);
        }

        public int CompareTo(Initializer obj)
        {
            if (obj == null) return 1;

            if (obj.Order > this.Order) return -1;
            if (obj.Order < this.Order) return 1;

            return 0;
        }

        public bool Equals(Initializer obj)
        {
            return obj.Order == this.Order;
        }
        #endregion
    }
}
