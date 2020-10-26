using System;
using WEngine;

namespace Winecrash
{
    public class ChunkBoxCollisionProvider : ICollisionProvider<Chunk, BoxCollider>
    {
        public bool Collide(Chunk c, BoxCollider b, out Vector3D force)
        {
            force = Vector3D.Zero;
            Vector3D bc = b.Center;
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
                cc.Z + ce.Z < bc.Z + be.Z    // if too much forward
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
                    World.GlobalToLocal(bc + be, out _, out Vector3I maxBpos);
                    // min coords
                    World.GlobalToLocal(bc - be, out _, out Vector3I minBpos);

                    bool collision = false;
                    int blockAmount = 0;

                    for (int z = WMath.Clamp(minBpos.Z, 0, Chunk.Depth - 1); z <= WMath.Clamp(maxBpos.Z, 0, Chunk.Depth - 1); z++)
                    {
                        for (int y = WMath.Clamp(minBpos.Y, 0, Chunk.Height - 1); y <= WMath.Clamp(maxBpos.Y, 0, Chunk.Height - 1); y++)
                        {
                            for (int x = WMath.Clamp(minBpos.X, 0, Chunk.Width - 1); x <= WMath.Clamp(maxBpos.X, 0, Chunk.Width - 1); x++)
                            {
                                if (!c[x, y, z].Transparent)
                                {
                                    collision = true;
                                    blockAmount++;
                                    
                                    Vector3D blockExtends = new Vector3D(0.5D);
                                    Vector3D blockCenter = new Vector3D(x + c.Coordinates.X * Chunk.Width,y,z + c.Coordinates.Y * Chunk.Depth) + blockExtends;

                                    /*double boxYBot = bc.Y - be.Y;
                                    double blockYTop = blockCenter.Y + blockExtends.Y;

                                    double delta = blockYTop - boxYBot;*/

                                    double posYDelta = (blockCenter.Y + blockExtends.Y) - (bc.Y - be.Y);
                                    
                                    
                                    Vector3D deltaBox = bc  - blockCenter;

                                    /*force.X = deltaBox.X > 0 ? Math.Max(deltaBox.X, force.X) : ;
                                    force.Y = Math.Max(deltaBox.Y, force.Z);
                                    force.Z = Math.Max(deltaBox.Z, force.Y);*/
                                    force += deltaBox;
                                }
                            }
                        }
                    }
                    
                    return collision;
                //}
                /*else //the origin is in a solid block => collision
                {
                    force = Vector3D.One * 3;
                    return true;
                }*/
                return false;
            }
        }
    }
}