using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Winecrash.Engine
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class Initializer : Attribute, IComparable, IComparable<Initializer>, IEquatable<Initializer>
    {
        public int Order { get; }
        private InitializerCallback Method;

        public delegate void InitializerCallback();


        private static bool Initialized = false;

        public Initializer()
        {
            this.Order = 0;
        }
        public Initializer(int order)
        {
            this.Order = order;
        }

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
                    throw new NotImplementedException("TODO: Logging\nError: " + e);
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
