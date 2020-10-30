using System;
using System.Security.Cryptography;
using WEngine;

namespace Winecrash
{
    public class ChunkBoxCollisionProvider : ICollisionProvider<Chunk, BoxCollider>
    {
        public bool Collide(Chunk c, BoxCollider b, out Vector3D translation, out Vector3D force)
        {
            force = Vector3D.Zero;
            translation = Vector3D.Zero;

            RigidBody rb = b.WObject.GetModule<RigidBody>();

            Vector3D bc = b.Center + rb.Velocity * Time.FixedDeltaTime;
            Vector3D be = b.Extents;


            Vector3D cc = c.WObject.Position +
                          Vector3D.Up * Chunk.Height / 2.0D +
                          Vector3D.Right * Chunk.Width / 2.0D +
                          Vector3D.Forward * Chunk.Depth / 2.0D;
            Vector3D ce = Vector3D.One * new Vector3D(Chunk.Width, Chunk.Height, Chunk.Depth) / 2.0D;

            // if outside of chunk, no collision.
            if ( //Y
                cc.Y - ce.Y > bc.Y + be.Y || // if too low
                cc.Y + ce.Y < bc.Y - be.Y || // if too high

                //X
                cc.X - ce.X > bc.X + be.X || // if too much left
                cc.X + ce.X < bc.X - be.X || // if too much right

                //Z
                cc.Z - ce.Z > bc.Z + be.Z || // if too much behind
                cc.Z + ce.Z < bc.Z - be.Z // if too much forward
            )
            {
                return false;
            }
            else // the box collider is within the chunk
            {
                /*World.GlobalToLocal(bc, out _, out Vector3I bpos);

                if (c[bpos.X, bpos.Y, bpos.Z].Transparent)
                {*/
                // if center is still transparent, check for every block the box
                // collider overlaps if it is solid or not.

                // max coords

                Vector3D max = bc + be;
                Vector3D min = bc - be;

                Vector3D cmax = new Vector3D(
                    WMath.Clamp(max.X, c.Coordinates.X * Chunk.Width,
                        c.Coordinates.X * Chunk.Width + (Chunk.Width - 1)),
                    WMath.Clamp(max.Y, 0, 255),
                    WMath.Clamp(max.Z, c.Coordinates.Y * Chunk.Depth, c.Coordinates.Y * Chunk.Depth + (Chunk.Depth - 1))
                );

                Vector3D cmin = new Vector3D(
                    WMath.Clamp(min.X, c.Coordinates.X * Chunk.Width,
                        c.Coordinates.X * Chunk.Width + (Chunk.Width - 1)),
                    WMath.Clamp(min.Y, 0, 255),
                    WMath.Clamp(min.Z, c.Coordinates.Y * Chunk.Depth, c.Coordinates.Y * Chunk.Depth + (Chunk.Depth - 1))
                );

                World.GlobalToLocal(cmax, out _, out Vector3I maxBpos);
                World.GlobalToLocal(cmin, out _, out Vector3I minBpos);

                bool collision = false;
                int blockAmount = 0;

                int yBlockAmount = 0;

                bool ignoreNegY = false;
                bool ignorePosY = false;
                
                for (int z = minBpos.Z; z <= maxBpos.Z; z++)
                {
                    for (int y = minBpos.Y; y <= maxBpos.Y; y++)
                    {
                        for (int x = minBpos.X; x <= maxBpos.X; x++)
                        {
                            if (!c[x, y, z].Transparent)
                            {
                                Vector3D blockExtends = new Vector3D(0.5D);
                                Vector3D blockCenter = new Vector3D(x + c.Coordinates.X * Chunk.Width, y,
                                    z + c.Coordinates.Y * Chunk.Depth) + blockExtends;

                                Vector3D bTrans;
                                
                                
                                
                                if (rb.Velocity.Y < 0.0 && bc.Y - be.Y < blockCenter.Y + blockExtends.Y && bc.Y - be.Y > blockCenter.Y)
                                {
                                    double deltaTop = (blockCenter.Y + blockExtends.Y) - (bc.Y - be.Y);

                                    //force += new Vector3D(0, -rb.Velocity.Y, 0);
                                    bTrans = new Vector3D(0, deltaTop + rb.Velocity.Y * Time.FixedDeltaTime, 0);
                                    
                                    translation += bTrans;
                                    bc += bTrans;
                                    ignoreNegY = true;

                                    continue;
                                } 
                                if (rb.Velocity.Y > 0.0 && bc.Y + be.Y > blockCenter.Y - blockExtends.Y &&
                                    bc.Y + be.Y < blockCenter.Y)
                                {
                                    double deltaTop = (bc.Y + be.Y) + (blockCenter.Y - blockExtends.Y);
                                    
                                    bTrans = new Vector3D(0, deltaTop, 0);
                                    
                                    translation += bTrans;
                                    bc += bTrans;
                                    ignorePosY = true;

                                    continue;
                                }
                                
                                if (rb.Velocity.X < 0.0 && bc.X - be.X < blockCenter.X + blockExtends.X)
                                {
                                    double deltaTop = (blockCenter.X + blockExtends.X) - (bc.X - be.X);

                                    //force += new Vector3D(0, -rb.Velocity.Y, 0);
                                    bTrans = new Vector3D(deltaTop + rb.Velocity.X * Time.FixedDeltaTime, 0, 0);
                                    
                                    translation += bTrans;
                                    bc += bTrans;

                                    continue;
                                }
                                
                                

                                /*if (rb.Velocity.Z < 0.0 && bc.Z - be.Z < blockCenter.Z + blockExtends.Z)
                                {
                                    double deltaTop = (blockCenter.Z + blockExtends.Z) - (bc.Z - be.Z);

                                    //force += new Vector3D(0, -rb.Velocity.Y, 0);
                                    bTrans = new Vector3D(0, 0, deltaTop + rb.Velocity.Z * Time.FixedDeltaTime);
                                    
                                    translation += bTrans;
                                    bc += bTrans;

                                    continue;
                                }*/

                                /*if (bc.Y + be.Y > blockCenter.Y - blockExtends.Y)
                                {
                                    double deltaBot = (blockCenter.Y - blockExtends.Y) - (bc.Y + be.Y);
                                    bTrans = new Vector3D(0, deltaBot, 0);

                                    translation += bTrans;
                                    bc += bTrans;

                                    continue;
                                }*/


                                /*Vector3D blockExtends = new Vector3D(0.5D);
                                Vector3D blockCenter = new Vector3D(x + c.Coordinates.X * Chunk.Width, y,
                                    z + c.Coordinates.Y * Chunk.Depth) + blockExtends;

                                Vector3D rel = bc - blockCenter;
                                Vector3D dir = rel.Normalized;

                                ///// Y Physics

                                Debug.Log(rel.Y - be.Y);
                                // box collider is higher than the block, repulse up
                                if (rel.Y - be.Y > blockExtends.Y && bc.Y > blockCenter.Y)
                                {
                                    double deltaTop = (blockCenter.Y + blockExtends.Y) - (bc.Y - be.Y);

                                    force = new Vector3D(0, -rb.Velocity.Y, 0);
                                    translation = new Vector3D(0, deltaTop + rb.Velocity.Y * Time.FixedDeltaTime, 0);
                                    bc += new Vector3D(0, deltaTop + rb.Velocity.Y * Time.FixedDeltaTime, 0);

                                    collisionOccured = true;
                                }

                                // repulse down
                                else if (rel.Y > -blockExtends.Y && bc.Y < blockCenter.Y)
                                {
                                    double deltaBot = (bc.Y + be.Y) - (blockCenter.Y - blockExtends.Y);

                                    force = new Vector3D(0, -rb.Velocity.Y, 0);
                                    translation = new Vector3D(0, deltaBot, 0);
                                    bc += new Vector3D(0, deltaBot, 0);

                                    collisionOccured = true;
                                }


                                ///// X Physics
                                // box is more east than the block

                                //if (bc.X > blockCenter.X)
                                //{

                                //    Debug.Log("EASTT");
                                //    double deltaEast = (blockCenter.X - blockExtends.X) - (bc.X + be.X);

                                //    force += new Vector3D(-rb.Velocity.X, 0, 0);

                                //    translation += new Vector3D(deltaEast, 1000, 0);

                                //    collisionOccured = true;
                                //}*/
                            }
                        }
                    }
                }
                
                if (translation.X != 0)
                {
                    force += new Vector3D(-rb.Velocity.X, 0, 0);
                }

                if (translation.Y != 0)
                {
                    force += new Vector3D(0, -rb.Velocity.Y, 0);
                }
                
                if (translation.Z != 0)
                {
                    force += new Vector3D(0, 0, -rb.Velocity.Z);
                }

                return translation != Vector3D.Zero;
            }
        }
    }
}