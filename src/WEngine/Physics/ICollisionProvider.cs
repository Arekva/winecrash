using System;

namespace WEngine
{
    public interface ICollisionProvider<T, U> where T : ICollider where U : ICollider
    {
        /// <summary>
        /// Do two colliders are currently colliding?
        /// </summary>
        /// <param name="first">The first collider</param>
        /// <param name="second">The second collider</param>
        /// <param name="force">The forced required to make both collider exclude each other.</param>
        /// <returns>Is there a collision?</returns>
        public bool Collide(T first, U second, out Vector3D translation, out Vector3D force);
    }
}