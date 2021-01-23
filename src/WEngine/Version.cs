using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace WEngine
{
    /// <summary>
    /// Structure defining a MAJOR.MINOR.PATCH version.
    /// </summary>
    public struct Version : IEquatable<Version>, IComparable<Version>
    {
        /// <summary>
        /// Major version.
        /// </summary>
        public uint Major { get; }
        
        /// <summary>
        /// Minor version.
        /// </summary>
        public uint Minor { get; }

        /// <summary>
        /// Patch version.
        /// </summary>
        public uint Patch { get; }

        /// <summary>
        /// The custom name of the version.
        /// </summary>
        [JsonIgnore] public string Name { get; }

        /// <summary>
        /// Create a new version.
        /// </summary>
        /// <param name="major">The major value of the version.</param>
        /// <param name="minor">The minor value of the version.</param>
        /// <param name="patch">The patch value of the version.</param>
        [JsonConstructor] public Version(uint major, uint minor, uint patch) : this(major, minor, patch, "Build") { }

        /// <summary>
        /// Create a new version.
        /// </summary>
        /// <param name="major">The major value of the version.</param>
        /// <param name="minor">The minor value of the version.</param>
        /// <param name="patch">The patch value of the version.</param>
        /// <param name="name">The name of the version. Optional.</param>
        public Version(uint major, uint minor, uint patch, string name = null) => (Major, Minor, Patch, Name) = (major, minor, patch, name);

        public override string ToString() => $"{Name} {Major}.{Minor}.{Patch}";

        public string ToString(string format) => format == null ? ToString() : format.Replace("{M}", Major.ToString()).Replace("{m}", Minor.ToString()).Replace("{p}", Patch.ToString()).Replace("{n}", Name);

        public override bool Equals(object obj) => obj is Version v && Equals(v);

        public bool Equals(Version v) => v.Major == this.Major && v.Minor == this.Minor && v.Patch == this.Patch;

        public override int GetHashCode()
        {
            int hashCode = -639545495;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + Patch.GetHashCode();
            return hashCode;
        }

        public int CompareTo(Version o)
        {
            if (this.Equals(o)) return 0;

            else
            {
                if (o.Major > this.Major) return -1;
                if (o.Minor > this.Minor) return -1;
                if (o.Patch > this.Patch) return -1;

                return 1;
            }
        }

        public static bool operator >(Version a, Version b) => a.CompareTo(b) > 0;

        public static bool operator <(Version a, Version b) => a.CompareTo(b) < 0;
    }
}
