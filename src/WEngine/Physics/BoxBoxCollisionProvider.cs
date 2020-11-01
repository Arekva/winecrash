using System;
using OpenTK.Graphics.OpenGL;

namespace WEngine
{
    public class BoxBoxCollisionProvider : ICollisionProvider<AABBCollider, AABBCollider>
    {
        /// <summary>
        /// Do a point collides with a box?
        /// </summary>
        /// <param name="colliding">The point to collide to the box collider</param>
        /// <param name="collider">The box collider the test happens on</param>
        /// <param name="translation">The translation required to move the point out</param>
        /// <param name="force">The force applied to the point when a collision happens. Always set to <see cref="Vector3D.Zero"/></param>
        /// <returns>Returns null if no collision, otherwise collision properties.</returns>
        // WORKING
        public Hit Collide(AABBCollider colliding, AABBCollider collider)
        {
            AABB box = colliding.AABB;
            AABB col = collider.AABB;
            
            double dx = box.Position.X - col.Position.X;
            double px = (box.Extents.X + col.Extents.X) - Math.Abs(dx);
            if (px <= 0.0) return new Hit();
            
            double dy = box.Position.Y - col.Position.Y;
            double py = (box.Extents.Y + col.Extents.Y) - Math.Abs(dy);
            if (py <= 0.0) return new Hit();
            
            double dz = box.Position.Z - col.Position.Z;
            double pz = (box.Extents.Z + col.Extents.Z) - Math.Abs(dz);
            if (pz <= 0.0) return new Hit();
            
            Hit hit = new Hit(collider);
            Vector3D hitDelta = hit.Delta;
            Vector3D hitNormal = hit.Normal;
            Vector3D hitPosition = hit.Position;
            
            if (px < py || px < pz)
            {
                int sx = Math.Sign(dx);
                hitDelta.X = px * sx;
                hitNormal.X = sx;
                hitPosition.X = col.Position.X + col.Extents.X * sx;
                hitPosition.Y = box.Position.Y;
                hitPosition.Z = box.Position.Z;
            }
            else if (py < px || py < pz)
            {
                int sy = Math.Sign(dy);
                hitDelta.Y = py * sy;
                hitNormal.Y = sy;
                hitPosition.X = box.Position.X;
                hitPosition.Y = col.Position.Y + col.Extents.Y * sy;
                hitPosition.Z = box.Position.Z;
            }
            else
            {
                int sz = Math.Sign(dz);
                hitDelta.Z = pz * sz;
                hitNormal.Z = sz;
                hitPosition.X = box.Position.X;
                hitPosition.Y = box.Position.Y;
                hitPosition.Z = col.Position.Z + col.Extents.Z * sz;
            }

            hit.Delta = hitDelta;
            hit.Normal = hitNormal;
            hit.Position = hitPosition;
            return hit;
        }

        public Sweep SweepCollide(AABBCollider colliding, Vector3D collidingVelocity, AABBCollider collider)
        {
            AABB box = colliding.AABB;
            AABB col = collider.AABB;
            Vector3D delta = collidingVelocity;
            
            Sweep sweep = new Sweep();

            if (delta == Vector3D.Zero)
            {
                sweep.Position = box.Position; 
                sweep.Hit = this.Collide(colliding, collider);
                if (sweep.Hit.HasHit)
                {
                    sweep.Hit.Time = 0.0D;
                    sweep.Time = 0.0D;
                }
                else
                {
                    sweep.Time = 1.0;
                }
                return sweep;
            }
            
            sweep.Hit = new RayBoxCollisionProvider().Collide(new Ray(box.Position, delta, delta.Length), collider, box.Extents);
    
            if (sweep.Hit.HasHit)
            {
                sweep.Time = WMath.Clamp(sweep.Hit.Time - Physics.Epsilon, 0, 1);
                sweep.Position = box.Position + delta * sweep.Time;

                Vector3D direction = delta.Normalized;

                sweep.Hit.Position = new Vector3D(
                    WMath.Clamp(sweep.Hit.Position.X + direction.X * box.Extents.X, col.Position.X - col.Extents.X,
                        col.Position.X + col.Extents.X),
                    WMath.Clamp(sweep.Hit.Position.Y + direction.Y * box.Extents.Y, col.Position.Y - col.Extents.Y,
                        col.Position.Y + col.Extents.Y),
                    WMath.Clamp(sweep.Hit.Position.Z + direction.Z * box.Extents.Z, col.Position.Z - col.Extents.Z,
                        col.Position.Z + col.Extents.Z)
                );
            }
            else
            {
                sweep.Position = box.Position + delta;
                sweep.Time = 1.0D;
            }

            
            return sweep;
        }
        
        public Sweep SweepCollideInto(AABBCollider colliding, Vector3D collidingVelocity, AABBCollider[] colliders)
        {
            //AABB box = collider.AABB;
            AABB col = colliding.AABB;
            Vector3D delta = collidingVelocity;

            Sweep nearest = new Sweep();
            nearest.Time = 1.0D;
            nearest.Position = col.Position + delta;

            for (int i = 0, il = colliders.Length; i < il; i++)
            {
                Sweep sweep = this.SweepCollide(colliding,delta,colliders[i]);
                if (sweep.Time < nearest.Time)
                {
                    nearest = sweep;
                }
            }
            

            return nearest;
        }
    }
}