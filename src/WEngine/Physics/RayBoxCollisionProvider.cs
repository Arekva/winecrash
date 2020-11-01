using System;

namespace WEngine
{
    public class RayBoxCollisionProvider : ICollisionProvider<Ray, AABBCollider>
    {
        /// <summary>
        /// Do a point collides with a box?
        /// </summary>
        /// <param name="colliding">The point to collide to the box collider</param>
        /// <param name="collider">The box collider the test happens on</param>
        /// <param name="translation">The translation required to move the point out</param>
        /// <param name="force">The force applied to the point when a collision happens. Always set to <see cref="Vector3D.Zero"/></param>
        /// <returns>Returns null if no collision, otherwise collision properties.</returns>
        public Hit Collide(Ray colliding, AABBCollider collider)
        {
            return this.Collide(colliding, collider, Vector3D.Zero);
        }
        
        public Hit Collide(Ray colliding, AABBCollider collider, Vector3D padding)
        {
            Vector3D pos = colliding.Origin;
            Vector3D delta = colliding.Direction * colliding.Length;
            
            AABB aabb = collider.AABB;


            Vector3D scale = new Vector3D(1.0D / delta.X, 1.0 / delta.Y, 1.0 / delta.Z);

            int signX = Math.Sign(scale.X);
            int signY = Math.Sign(scale.Y);
            int signZ = Math.Sign(scale.Z);
            
            double nearTimeX = (aabb.Position.X - signX * (aabb.Extents.X + padding.X) - pos.X) * scale.X;
            double nearTimeY = (aabb.Position.Y - signY * (aabb.Extents.Y + padding.Y) - pos.Y) * scale.Y;
            double nearTimeZ = (aabb.Position.Z - signZ * (aabb.Extents.Z + padding.Z) - pos.Z) * scale.Z;
            double farTimeX = (aabb.Position.X + signX * (aabb.Extents.X + padding.X) - pos.X) * scale.X;
            double farTimeY = (aabb.Position.Y + signY * (aabb.Extents.Y + padding.Y) - pos.Y) * scale.Y;
            double farTimeZ = (aabb.Position.Z + signZ * (aabb.Extents.Z + padding.Z) - pos.Z) * scale.Z;

            if (nearTimeX > farTimeY || nearTimeX > farTimeZ ||
                nearTimeY > farTimeX || nearTimeY > farTimeZ ||
                nearTimeZ > farTimeX || nearTimeZ > farTimeY)
                return new Hit();

            double nearTime = Math.Max(Math.Max(nearTimeX, nearTimeY), nearTimeZ);
            double farTime = Math.Max(Math.Max(farTimeX, farTimeY), farTimeZ);

            if (nearTime >= 1 || farTime <= 0)
                return new Hit();

            Hit hit = new Hit(collider)
            {
                Time = WMath.Clamp(nearTime, 0, 1)
            };

            if (nearTimeX > nearTimeY || nearTimeX > nearTimeZ)
            {
                hit.Normal.X = -signX;
                hit.Normal.Y = 0.0;
                hit.Normal.Z = 0.0;
            }
            else if (nearTimeY > nearTimeX || nearTimeY > nearTimeZ)
            {
                hit.Normal.X = 0.0;
                hit.Normal.Y = -signY;
                hit.Normal.Z = 0.0;
            }
            else 
            {
                hit.Normal.X = 0.0;
                hit.Normal.Y = 0.0;
                hit.Normal.Z = -signZ;
            }

            hit.Delta = -delta * (1.0 - hit.Time);

            hit.Position = pos + delta * hit.Time;

            return hit;
        }
    }
}