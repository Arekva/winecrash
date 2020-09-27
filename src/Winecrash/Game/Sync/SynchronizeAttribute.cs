using System;

namespace Winecrash
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SynchronizeAttribute : Attribute
    {
        public bool Sync { get; } = true;
    }
}
