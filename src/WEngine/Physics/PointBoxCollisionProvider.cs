using System;

namespace WEngine
{
    public class PointBoxCollisionProvider : ICollisionProvider<Vector3D, AABBCollider>
    {
        /// <summary>
        /// Do a point collides with a box?
        /// </summary>
        /// <param name="colliding">The point to collide to the box collider</param>
        /// <param name="collider">The box collider the test happens on</param>
        /// <param name="translation">The translation required to move the point out</param>
        /// <param name="force">The force applied to the point when a collision happens. Always set to <see cref="Vector3D.Zero"/></param>
        /// <returns>Returns null if no collision, otherwise collision properties.</returns>
        
        //WORKING !!
        public Hit Collide(Vector3D colliding, AABBCollider collider)
        {
            Vector3D point = colliding;
            AABB aabb = collider.AABB;
            
            double dx = point.X - aabb.Position.X;
            double px = aabb.Extents.X - Math.Abs(dx);
            if (px <= 0) return new Hit();
            
            double dy = point.Y - aabb.Position.Y;
            double py = aabb.Extents.Y - Math.Abs(dy);
            if (py <= 0) return new Hit();
            
            
            double dz = point.Z - aabb.Position.Z;
            double pz = aabb.Extents.Z - Math.Abs(dz);
            if (pz <= 0) return new Hit();
            
            Hit hit = new Hit(collider);
            Vector3D hitDelta = hit.Delta;
            Vector3D hitNormal = hit.Normal;
            Vector3D hitPosition = hit.Position;

            /*Axis smallestOverlap = Axis.Y;

            if (px < py && px < pz) smallestOverlap = Axis.X;
            else if (py < px && py < pz) smallestOverlap = Axis.Y;
            else if (pz < px && pz < py) smallestOverlap = Axis.Z;*/
            
            
            if (px < py && px < pz)
            {
                int sx = Math.Sign(dx);
                hitDelta.X = px * sx;
                hitNormal.X = sx;
                hitPosition.X = aabb.Position.X + aabb.Extents.X * sx;
                hitPosition.Y = point.Y;
                hitPosition.Z = point.Z;
            }
            else if (py < px && py < pz)
            {
                int sy = Math.Sign(dy);
                hitDelta.Y = py * sy;
                hitNormal.Y = sy;
                hitPosition.X = point.X;
                hitPosition.Y = aabb.Position.Y + aabb.Extents.Y * sy;
                hitPosition.Z = point.Z;
            }
            else
            {
                int sz = Math.Sign(dz);
                hitDelta.Z = pz * sz;
                hitNormal.Z = sz;
                hitPosition.X = point.X;
                hitPosition.Y = point.Y;
                hitPosition.Z = aabb.Position.Z + aabb.Extents.Z * sz;
            }
            
            hit.Delta = hitDelta;
            hit.Normal = hitNormal;
            hit.Position = hitPosition;
            return hit;
        }
    }
}