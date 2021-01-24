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
            return Collide(colliding, collider, Vector3D.Zero);
        }
        
        public Hit CollideNEW(Ray colliding, AABBCollider collider)
        {
            const double collidingDistance = 0.0001D;
            
            Vector3D vp = collider.Center;
            Vector3D vs = collider.Extents;
            Vector3D vd = vp - vs; 

            double x = Math.Max(0.0D, vd.X);
            double y = Math.Max(0.0D, vd.Y);
            double z = Math.Max(0.0D, vd.Z);
            
            double dist = x*x + y*y + z*z;

            if (dist <= collidingDistance)
            {
                Hit hit = new Hit(collider);
                hit.Normal = GetNormalNEW(collider, vp, vs);
                hit.Distance = Math.Sqrt(dist);
                hit.Position = new Vector3D(x,y,z);
                return hit;
            }
            else
            {
                return new Hit();
            }
        }

        private Vector3D GetNormalNEW(AABBCollider collider, Vector3D vp, Vector3D vs)
        {
            Vector3D n = Vector3D.Left;

            Vector3D rp = vp - collider.Center;
            
            if (rp.Y > vs.Y)
            {
                n.X = 0.0D;
                n.Y = 1.0D;
            }
            else if (rp.Y < -vs.Y)
            {
                n.X = 0.0D;
                n.Y = -1.0D;
            }

            // Defaults
            /*else if (rp.x > vs.x)
            {
                n.x = 1.0F;
            }*/
    
            else if (rp.X < -vs.X)
            {
                n.X = -1.0D;
            }
    
            else if (rp.Z > vs.Z)
            {
                n.X = 0.0D;
                n.Z = 1.0D;
            }

            else if (rp.Z < -vs.Z)
            {
                n.X = 0.0D;
                n.Z = -1.0D;
            }

            return n;
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
            
            Vector3D n = Vector3D.Zero;
            

            
            hit.Delta = -delta * (1.0 - hit.Time);
            hit.Position = pos + delta * hit.Time;
            hit.Normal = GetNormalFromPoint(collider, hit.Position);
            return hit;
        }
        
        public Vector3D GetNormalFromPoint(AABBCollider aabb, Vector3D point)        
        {            
            Vector3D normal = Vector3D.Zero;            
            double min = double.MaxValue;            
            double distance;            
            point -= aabb.Center;            
            distance = Math.Abs(aabb.Extents.X - Math.Abs(point.X));           
            if (distance < min)            
            {                
                min = distance;                
                normal = Vector3D.Right * Math.Sign(point.X);    // Cardinal axis for X
            }            
            distance = Math.Abs(aabb.Extents.Y - Math.Abs(point.Y));            
            if (distance < min)            
            {                
                min = distance;                
                normal = Vector3D.Up * Math.Sign(point.Y);    // Cardinal axis for Y
            }            
            distance = Math.Abs(aabb.Extents.Z - Math.Abs(point.Z));            
            if (distance < min)            
            {                
                min = distance;               
                normal = Vector3D.Forward * Math.Sign(point.Z);    // Cardinal axis for Z
            }            
            return normal;        
        }
    }
}