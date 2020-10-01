using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;

namespace Winecrash
{
    public enum BlockFaces
    {
        /// <summary>
        /// Refers to <see cref="Vector3F.Up"/>
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
        /// Refers to <see cref="Vector3F.Backward"/>
        /// </summary>
        South
    }

    public static class BlockFacesExtentions
    {
        /// <summary>
        /// Get a <see cref="Vector3D"/> direction from a <see cref="Directions"/> direction.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static BlockFaces Face(Vector3D dir)
        {
            if (dir == Vector3D.Up)
            {
                return BlockFaces.Up;
            }
            else if (dir == Vector3D.Down)
            {
                return BlockFaces.Down;
            }
            else if (dir == Vector3D.Forward)
            {
                return BlockFaces.South;
            }
            else if (dir == Vector3D.Backward)
            {
                return BlockFaces.North;
            }
            else if (dir == Vector3D.Right)
            {
                return BlockFaces.East;
            }
            else if (dir == Vector3D.Left)
            {
                return BlockFaces.West;
            }
            else
            {
                return BlockFaces.Up;
            }
        }

        /// <summary>
        /// Get a <see cref="Vector3D"/> direction from a <see cref="Directions"/> direction.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Vector3D Direction(this BlockFaces face)
        {
            switch (face)
            {
                case BlockFaces.Up: return Vector3D.Up;
                case BlockFaces.Down: return Vector3D.Down;

                case BlockFaces.North: return Vector3D.Backward;
                case BlockFaces.South: return Vector3D.Forward;

                case BlockFaces.West: return Vector3D.Left;
                case BlockFaces.East: return Vector3D.Right;

                default: return Vector3D.Up;
            }
        }
    }
}
