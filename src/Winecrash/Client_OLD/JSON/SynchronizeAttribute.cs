using System;

namespace Winecrash.Game
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SynchronizeAttribute : Attribute
    {
        public bool Sync { get; } = true;
    }
}
