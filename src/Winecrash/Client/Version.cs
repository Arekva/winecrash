using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Game
{
    /// <summary>
    /// Structure defining a MAJOR.MINOR.PATCH version.
    /// </summary>
    public struct Version : IEquatable<Version>
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
        public string Name { get; }

        /// <summary>
        /// Create a new version.
        /// </summary>
        /// <param name="major">The major value of the version.</param>
        /// <param name="minor">The minor value of the version.</param>
        /// <param name="patch">The patch value of the version.</param>
        /// <param name="name">The name of the version. Optional.</param>
        public Version(uint major, uint minor, uint patch, string name = null)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Name = name;
        }

        public override string ToString()
        {
            return $"{Name} {Major}.{Minor}.{Patch}";
        }

        public override bool Equals(object obj)
        {
            return obj is Version v && Equals(v);
        }

        public bool Equals(Version v)
        {
            return v.Major == this.Major && v.Minor == this.Minor && v.Patch == this.Patch;
        }

        public override int GetHashCode()
        {
            int hashCode = -639545495;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + Patch.GetHashCode();
            return hashCode;
        }
    }
}
