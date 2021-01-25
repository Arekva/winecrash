using System;
using System.Collections.Generic;
using WEngine;

namespace Winecrash
{
    public class ChunkBoxCollisionProvider : ICollisionProvider<Chunk, BoxCollider>
    {
        public Sweep SweepCollide(Chunk c, BoxCollider b)
        {
            //force = Vector3D.Zero;
            //translation = Vector3D.Zero;

            RigidBody rb = b.WObject.GetModule<RigidBody>();

            Vector3D bc = b.Center;
            Vector3D be = b.Extents * 4;


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
                return new Sweep();
            }
            else // the box collider is within the chunk
            {
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


                List<AABBCollider> colliders = new List<AABBCollider>();

                for (int z = minBpos.Z; z <= maxBpos.Z; z++)
                {
                    for (int y = minBpos.Y; y <= maxBpos.Y; y++)
                    {
                        for (int x = minBpos.X; x <= maxBpos.X; x++)
                        {
                            if (c[x, y, z].Collides)
                            {
                                Vector3D blockExtents = new Vector3D(0.5D);
                                Vector3D blockCenter = new Vector3D(x + c.Coordinates.X * Chunk.Width, y,
                                    z + c.Coordinates.Y * Chunk.Depth) + blockExtents;

                                FreeBoxCollider collider = new FreeBoxCollider()
                                {
                                    Center = blockCenter,
                                    Extents = blockExtents
                                };

                                colliders.Add(collider);
                            }
                        }
                    }
                }

                Sweep resultingSweep =
                    new BoxBoxCollisionProvider().SweepCollideInto(b, rb.Velocity * Time.PhysicsDelta,
                        colliders.ToArray());

                for (int i = 0; i < colliders.Count; i++)
                {
                    colliders[i].Delete();
                }

                colliders.Clear();
                colliders = null;

                return resultingSweep;
            }
        }

        public static RaycastChunkHit CollideWorld(BoxCollider b)
        {
            // ok donc ça fais 3 jours non stop que j'essai de bosser sur les collisions et ça me fait vraiment chier.
            // du coup je vais reprendre celles que j'avais fait à la zbeul pour la Predev, c'est crade mais au
            // moins ça fonctionne bordel de merde.

            if (!b || !b.Enabled) return new RaycastChunkHit();

            RigidBody rb = b.WObject.GetModule<RigidBody>();
            if (!rb)
            {
                Debug.LogWarning(
                    "No collision possible between a chunk and a box collider with no RigidBody. Please add a Rigidbody module.");
                return new RaycastChunkHit();
            }
            
            RaycastChunkHit hit = new RaycastChunkHit();

            Vector3D v = rb.Velocity;
            Vector3D originalVelocity = v;
            Vector3D c = b.Center;
            
            double castExtents = b.CastExtents;
            double castLength = b.CastLength;
            
            RaycastChunkHit CheckRight()
            {
                if (v.X > 0.0D) return new RaycastChunkHit();

                //front up
                hit = RaycastChunk(
                    new Ray(
                        c + Vector3D.Up * b.Extents.Y * castExtents +
                        Vector3D.Left * b.Extents.X +
                        Vector3D.Forward * b.Extents.Z * castExtents,
                        Vector3D.Right, castLength), castLength);

                if (!hit.HasHit)
                    //front down
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Down * b.Extents.Y * castExtents +
                            Vector3D.Left * b.Extents.X +
                            Vector3D.Forward * b.Extents.Z * castExtents,
                            Vector3D.Right, castLength), castLength);
                if (!hit.HasHit)
                    //back down
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Down * b.Extents.Y * castExtents +
                            Vector3D.Left * b.Extents.X +
                            Vector3D.Backward * b.Extents.Z * castExtents,
                            Vector3D.Right, castLength), castLength);
                if (!hit.HasHit)
                    //back up
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Up * b.Extents.Y * castExtents +
                            Vector3D.Left * b.Extents.X +
                            Vector3D.Backward * b.Extents.Z * castExtents,
                            Vector3D.Right, castLength), castLength);

                if (hit.HasHit)
                {
                    Vector3D mask = new Vector3D(0,1,1);
                    c *= mask;
                    v *= mask;
                    
                    c += Vector3D.Right * (hit.GlobalBlockPosition.X + b.Extents.X + 1.0D);
                }

                return hit;
            }
            RaycastChunkHit CheckLeft()
            {
                if (v.X < 0.0D) return new RaycastChunkHit();

                //front up
                hit = RaycastChunk(
                    new Ray(
                        c + Vector3D.Up * b.Extents.Y * castExtents +
                        Vector3D.Right * b.Extents.X +
                        Vector3D.Forward * b.Extents.Z * castExtents,
                        Vector3D.Left, castLength), castLength);

                if (!hit.HasHit)
                    //front down
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Down * b.Extents.Y * castExtents +
                            Vector3D.Right * b.Extents.X +
                            Vector3D.Forward * b.Extents.Z * castExtents,
                            Vector3D.Left, castLength), castLength);
                if (!hit.HasHit)
                    //back down
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Down * b.Extents.Y * castExtents +
                            Vector3D.Right * b.Extents.X +
                            Vector3D.Backward * b.Extents.Z * castExtents,
                            Vector3D.Left, castLength), castLength);
                if (!hit.HasHit)
                    //back up
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Up * b.Extents.Y * castExtents +
                            Vector3D.Right * b.Extents.X +
                            Vector3D.Backward * b.Extents.Z * castExtents,
                            Vector3D.Left, castLength), castLength);

                if (hit.HasHit)
                {
                    Vector3D mask = new Vector3D(0, 1, 1);
                    c *= mask;
                    v *= mask;

                    c += Vector3D.Right * (hit.GlobalBlockPosition.X - b.Extents.X);
                }

                return hit;
            }
            RaycastChunkHit CheckForward()
            {
                if (v.Z < 0.0D) return new RaycastChunkHit();
                //front up
                hit = RaycastChunk(
                    new Ray(c + Vector3D.Up * b.Extents.Y * castExtents + 
                            Vector3D.Right * b.Extents.X * castExtents + 
                            Vector3D.Forward * b.Extents.Z, 
                        Vector3D.Forward, castLength), castLength);
                
                // front down
                if (!hit.HasHit)
                    hit = RaycastChunk(
                        new Ray(c + Vector3D.Down * b.Extents.Y * castExtents +
                                Vector3D.Right * b.Extents.X * castExtents + 
                                Vector3D.Forward * b.Extents.Z, 
                                Vector3D.Forward, castLength), castLength);
                //back down
                if (!hit.HasHit)
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Down * b.Extents.Y * castExtents +
                            Vector3D.Left * b.Extents.X * castExtents +
                            Vector3D.Forward * b.Extents.Z,
                            Vector3D.Forward, castLength), castLength);
                if (!hit.HasHit)
                    //back up
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Up * b.Extents.Y * castExtents +
                            Vector3D.Left * b.Extents.X * castExtents +
                            Vector3D.Forward * b.Extents.Z,
                            Vector3D.Forward, castLength), castLength);

                if (hit.HasHit)
                {
                    c *= new Vector3D(1, 1, 0);
                    c += Vector3D.Forward * (hit.GlobalBlockPosition.Z - b.Extents.Z);
                    v *= new Vector3D(1, 1, 0);
                }

                return hit;
            }
            RaycastChunkHit CheckBackward()
            {
                if (v.Z > 0.0D) return new RaycastChunkHit();

                //front up
                hit = RaycastChunk(
                    new Ray(
                        c + Vector3D.Up * b.Extents.Y * castExtents +
                        Vector3D.Right * b.Extents.X * castExtents +
                        Vector3D.Backward * b.Extents.Z,
                        Vector3D.Backward, castLength), castLength);

                if (!hit.HasHit)
                    //front down
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Down * b.Extents.Y * castExtents +
                            Vector3D.Right * b.Extents.X * castExtents +
                            Vector3D.Backward * b.Extents.Z,
                            Vector3D.Backward, castLength), castLength);
                if (!hit.HasHit)
                    //back down
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Down * b.Extents.Y * castExtents +
                            Vector3D.Left * b.Extents.X * castExtents +
                            Vector3D.Backward * b.Extents.Z,
                            Vector3D.Backward, castLength), castLength);
                if (!hit.HasHit)
                    //back up
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Up * b.Extents.Y * castExtents +
                            Vector3D.Left * b.Extents.X * castExtents +
                            Vector3D.Backward * b.Extents.Z,
                            Vector3D.Backward, castLength), castLength);

                if (hit.HasHit)
                {
                    Vector3D mask = new Vector3D(1,1,0);
                    c *= mask;
                    v *= mask;
                    
                    c += Vector3D.Forward * (hit.GlobalBlockPosition.Z + b.Extents.Z + 1.0D);
                }

                return hit;
            }
            RaycastChunkHit CheckGrounded()
            {
                if (v.Y > 0) return new RaycastChunkHit();

                //front right
                    hit = RaycastChunk(new Ray(c + Vector3D.Down * b.Extents.Y + Vector3D.Right * b.Extents.X * castExtents + Vector3D.Forward * b.Extents.Z * castExtents, Vector3D.Down, castLength), castLength);

                //front left
                if (!hit.HasHit) 
                    hit = RaycastChunk(new Ray(c + Vector3D.Down * b.Extents.Y + Vector3D.Left * b.Extents.X * castExtents + Vector3D.Forward * b.Extents.Z * castExtents, Vector3D.Down, castLength),castLength);

                //back left
                if (!hit.HasHit) 
                    hit = RaycastChunk(new Ray(c + Vector3D.Down * b.Extents.Y + Vector3D.Left * b.Extents.X * castExtents + Vector3D.Backward * b.Extents.Z * castExtents, Vector3D.Down, castLength), castLength);

                //back right
                if (!hit.HasHit) 
                    hit = RaycastChunk(new Ray(c + Vector3D.Down * b.Extents.Y + Vector3D.Right * b.Extents.X * castExtents + Vector3D.Backward * b.Extents.Z * castExtents, Vector3D.Down, castLength), castLength);


                if (hit.HasHit)
                {
                    Vector3D mask = new Vector3D(1,0,1);
                    c *= mask;
                    v *= mask;
                    
                    // set to block top position and add extents
                    c += Vector3D.Up * (hit.LocalPosition.Y + 1.0D + b.Extents.Y);
                }

                return hit;
            }
            RaycastChunkHit CheckCeiling()
            {
                if (v.Y < 0) return new RaycastChunkHit();

                //front right
                hit = RaycastChunk(
                    new Ray(
                        c + Vector3D.Up * b.Extents.Y +
                        Vector3D.Right * b.Extents.X * castExtents +
                        Vector3D.Forward * b.Extents.Z * castExtents,
                        Vector3D.Up, castLength), castLength);


                //front left
                if (!hit.HasHit)
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Up * b.Extents.Y +
                            Vector3D.Left * b.Extents.X * castExtents +
                            Vector3D.Forward * b.Extents.Z * castExtents,
                            Vector3D.Up, castLength), castLength);

                //back left
                if (!hit.HasHit)
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Up * b.Extents.Y +
                            Vector3D.Left * b.Extents.X * castExtents +
                            Vector3D.Backward * b.Extents.Z * castExtents,
                            Vector3D.Up, castLength), castLength);

                //back right
                if (!hit.HasHit)
                    hit = RaycastChunk(
                        new Ray(
                            c + Vector3D.Up * b.Extents.Y +
                            Vector3D.Right * b.Extents.X * castExtents +
                            Vector3D.Backward * b.Extents.Z * castExtents,
                            Vector3D.Up, castLength), castLength);


                if (hit.HasHit)
                {
                    Vector3D mask = new Vector3D(1, 0, 1);
                    c *= mask;
                    v *= mask;

                    // set to block top position and add extents
                    c += Vector3D.Up * (hit.LocalPosition.Y - b.Extents.Y);
                }

                return hit;
            }

            CheckRight();
            CheckLeft();
            CheckForward();
            CheckBackward();
            CheckCeiling();
            CheckGrounded();
            
            b.Center = c;
            //Vector3D finalVelocity = Vector3D.Zero;

            /*if (v.X != 0) finalVelocity.X = originalVelocity.X;
            if (v.Y != 0) finalVelocity.Y = originalVelocity.Y;
            if (v.Z != 0) finalVelocity.Z = originalVelocity.Z;*/
            
            rb.Velocity = v;

            return hit;
        }

        /// <summary>
        /// Shit script, do not use. However still used for entity physics ¯\_(ツ)_/¯
        /// </summary>
        /// <param name="ray">The ray to cast the chunk.</param>
        /// <param name="precision">The ray precision, try to keep the number high
        /// high, otherwise the pc will die.</param>
        /// <returns>The hit status of the ray'cast'.</returns>
        public static RaycastChunkHit RaycastChunk(Ray ray, double precision = 0.1D)
        {
            Vector3D pos = ray.Origin;
            double distance = 0.0D;
            double length = ray.Length;

            Vector2I cpos;
            Vector3I bpos;

            Chunk chunk = null;

            Block block = null;

            while (distance < length)
            {
                World.GlobalToLocal(pos, out cpos, out bpos);


                chunk = World.GetChunk(cpos, "winecrash:overworld");

                if (chunk != null 
                    && bpos.X > -1 && bpos.X < Chunk.Width
                    && bpos.Y > -1 && bpos.Y < Chunk.Height
                    && bpos.Z > -1 && bpos.Z < Chunk.Depth)
                {
                    block = chunk[bpos.X, bpos.Y, bpos.Z];

                    if (block.Collides)
                    {
                        Vector3I blockGlobalUnitPosition =
                            new Vector3I(bpos.X + cpos.X * 16, bpos.Y, bpos.Z + cpos.Y * 16);

                        Vector3D blockGlobalPosition = (Vector3D) blockGlobalUnitPosition + Vector3D.One * 0.5D;

                        Vector3D rp = pos - blockGlobalPosition;

                        Vector3D n = new Vector3D();

                        //up
                        if (rp.Y > Math.Abs(rp.X) && rp.Y > Math.Abs(rp.Z))
                        {
                            n.X = 0.0;
                            n.Y = 1.0;
                            n.Z = 0.0;
                        }
                        //down
                        else if (rp.Y < Math.Abs(rp.X) * -1 && rp.Y < Math.Abs(rp.Z) * -1)
                        {
                            n.X = 0.0;
                            n.Y = -1.0;
                            n.Z = 0.0;
                        }
                        //east
                        else if (rp.X > Math.Abs(rp.Z))
                        {
                            n.X = 1.0;
                            n.Y = 0.0;
                            n.Z = 0.0;
                        }
                        //west
                        else if (rp.X < Math.Abs(rp.Z) * -1)
                        {
                            n.X = -1.0;
                            n.Y = 0.0;
                            n.Z = 0.0;
                        }
                        //North
                        else if (rp.Z > 0.0)
                        {
                            n.X = 0.0;
                            n.Y = 0.0;
                            n.Z = 1.0;
                        }
                        //South
                        else
                        {
                            n.X = 0.0;
                            n.Y = 0.0;
                            n.Z = -1.0;
                        }

                        return new RaycastChunkHit(pos, n, distance, block, chunk, bpos, true);
                    }
                }

                else
                {
                    return new RaycastChunkHit();
                }

                pos += ray.Direction * precision;
                distance += precision;
            }

            return new RaycastChunkHit(pos, Vector3D.Up, distance, block, chunk, Vector3I.Zero, false);
        }


        public Hit Collide(Chunk c, BoxCollider b) => throw new NotImplementedException();
    }
}