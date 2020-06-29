using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Winecrash.Client
{
    public enum BlockFaces
    {
        /// <summary>
        /// Refers to <see cref="Vector3F.up"/>
        /// </summary>
        Up,
        /// <summary>
        /// Refers to <see cref="Vector3F.Down"/>
        /// </summary>
        Down,
        /// <summary>
        /// Refers to <see cref="Vector3F.Left"/>
        /// </summary>
        West,
        /// <summary>
        /// Refers to <see cref="Vector3F.Right"/>
        /// </summary>
        East,
        /// <summary>
        /// Refers to <see cref="Vector3F.Forward"/>
        /// </summary>
        North,
        /// <summary>
        /// Refers to <see cref="Vector3F.Back"/>
        /// </summary>
        South
    }
}
