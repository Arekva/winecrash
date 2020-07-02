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

    public static class BlockFacesExtentions
    {
        public static BlockFaces Face(this Vector3D dir)
        {
            dir.Normalize();

            //Up / Down
            if (dir.Y > 0.5D) return BlockFaces.Up;
            else if (dir.Y < -0.5D) return BlockFaces.Down;

            //North / South
            if (dir.Z > 0.5D) return BlockFaces.North;
            else if (dir.Z < -0.5D) return BlockFaces.South;

            //East / West
            if (dir.X > 0.5D) return BlockFaces.East;
            else if (dir.X < -0.5D) return BlockFaces.West;


            return BlockFaces.Up;
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

                case BlockFaces.North: return Vector3D.Forward;
                case BlockFaces.South: return Vector3D.Backward;

                case BlockFaces.West: return Vector3D.Left;
                case BlockFaces.East: return Vector3D.Right;

                default: return Vector3D.Up;
            }
        }
    }
}
