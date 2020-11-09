using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LibNoise.Combiner;
using WEngine;

namespace Winecrash
{
    public class ChunkRayCollisionProvider : ICollisionProvider<Chunk, Ray>
    {
        public Hit Collide(Chunk colliding, Ray collider)
        {
            throw new NotImplementedException();
        }

        public RaycastChunkHit Raycast(Chunk chunk, Ray ray)
        {
            //World.GlobalToLocal(ray.Origin, out Vector2I minCPos, out Vector3I minBPos);
            //World.GlobalToLocal(ray.Origin + ray.Direction * ray.Length, out Vector2I maxCPos, out Vector3I maxBPos);
            
            Vector3I minRelDir = World.RelativePositionToChunk(ray.Origin, chunk.Coordinates); 
            Vector3I maxRelDir = World.RelativePositionToChunk(ray.Origin + ray.Direction * ray.Length, chunk.Coordinates);

            int minX = Math.Min(minRelDir.X, maxRelDir.X);
            int maxX = Math.Max(minRelDir.X, maxRelDir.X);
            
            int minY = Math.Min(minRelDir.Y, maxRelDir.Y);
            int maxY = Math.Max(minRelDir.Y, maxRelDir.Y);
            
            int minZ = Math.Min(minRelDir.Z, maxRelDir.Z);
            int maxZ = Math.Max(minRelDir.Z, maxRelDir.Z);

            RayBoxCollisionProvider provider = new RayBoxCollisionProvider();
            
            List<RaycastChunkHit> hits = new List<RaycastChunkHit>();

            for (int z = minZ; z <= maxZ; z++)
            {
                if(z < 0 || z > Chunk.Depth - 1) continue;
                
                for (int y = minY; y <= maxY; y++)
                {
                    if(y < 0 || y > Chunk.Height - 1) continue;
                    
                    for (int x = minX; x <= maxX; x++)
                    {
                        if (x < 0 || x > Chunk.Width - 1) continue;

                        Block b = chunk[x, y, z];
                        
                        if (b.Identifier != "winecrash:air")
                        {

                            Vector3D blockExtents = new Vector3D(0.5D);
                            Vector3D blockCenter = new Vector3D(x + chunk.Coordinates.X * Chunk.Width, y,
                                z + chunk.Coordinates.Y * Chunk.Depth) + blockExtents;

                            FreeBoxCollider collider = new FreeBoxCollider()
                            {
                                Center = blockCenter,
                                Extents = blockExtents
                            };

                            Hit h = provider.Collide(ray, collider);
                            
                            collider.Delete();

                            if (h.HasHit)
                            {
                                hits.Add(new RaycastChunkHit(h.Position, h.Normal, h.Time * ray.Length, b, chunk,
                                    new Vector3I(x, y, z), true));
                            }
                        }
                    }
                }
            }

            if (hits.Count != 0)
            {
                return hits.OrderBy(h => h.Distance).First();
            }
            
            return new RaycastChunkHit();
        }
    }
}