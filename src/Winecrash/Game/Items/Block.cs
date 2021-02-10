using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;
using Winecrash.Entities;

namespace Winecrash
{
    public class Block : Item
    {
        //-- set in item config --//
        public bool Transparent { get; set; } = false;
        //-- set in item config --//
        public bool Collides { get; set; } = false;
        //-- set in item config --//
        public double DigTime { get; set; } = 1.0D;

        //-- set in item config --//
        public bool DrawInternalFaces { get; set; } = false;
        
        //-- set in item config --//
        public bool DrawAllSides { get; set; } = false;

        //-- set in block config --//
        public PhysicMaterial Physics { get; set; } = new PhysicMaterial(1.0, 0.0);
        
        //-- set in harvest config--//
        public List<HarvestTable> HarvestTables { get; set; } = new List<HarvestTable>(1);
        
        public virtual void MainInteraction() {}
        public virtual void SecondaryInteraction() {}

        public virtual void Harvest(Chunk chunk, int x, int y, int z)
        {
            ItemAmount[] items = NextHarvestDrop(); // no custom chance for now
            for (int i = 0; i < items.Length; i++)
            {
                ItemEntity item = ItemEntity.FromItem(ItemCache.Get(items[i].Identifier), items[i].Amount);
                item.WObject.Position = (Vector3D)World.LocalToGlobal(chunk.Coordinates, new Vector3I(x, y, z)) + Vector3D.One * 0.5D;
            
                // apply force to item
                Vector2D dir = Winecrash.Random.NextDirection2D();
                Vector3D force = new Vector3D(dir.X, 0.0, dir.Y) * Item.ItemDigSpawnForce;

                item.RigidBody.Velocity += force;
            }
        }

        public ItemAmount[] NextHarvestDrop(double minChance = 0.0, double maxChance = 1.0)
        {
            // sanitize inputs
            if (minChance < 0.0 || minChance > 1.0) throw new ArgumentOutOfRangeException(nameof(minChance), minChance, "The minimal drop chance must be in the [0.0, 1.0] range.");
            if (maxChance < 0.0 || maxChance > 1.0) throw new ArgumentOutOfRangeException(nameof(maxChance), maxChance, "The maximal drop chance must be in the [0.0, 1.0] range.");
            if (minChance > maxChance) throw new ArgumentOutOfRangeException(nameof(minChance), minChance, "The minimal drop chance must be lower than the maximal one.");
            
            
            ItemAmount[] results = null;
            
            // if no table, return an empty array *but not null*
            if (HarvestTables.Count > 0)
            {
                //if (HarvestTables.Count == 1) results = HarvestTables[0].Results;
                if (minChance == 1.0) results = HarvestTables.Last().Results;
                else
                {
                    double selected = Winecrash.Random.NextDouble();
                    // if needed move the selected chance range to the minimal chance one
                    if (minChance != 0 || maxChance != 0) selected = WMath.Remap(selected, 0.0, 1.0, minChance, maxChance);
                    // invert the range in order to get the least chance
                    
                    // this will take the one with the least chance to appear
                    // (considering the tables are sorted by chance, cf HarvestTable's constructor)
                    HarvestTable table = HarvestTables.FirstOrDefault(ht => ht.Chance >= selected);
                    if (table.Block != null) results = table.Results;
                }
            }

            return results ?? Array.Empty<ItemAmount>();
        }
        

        public void Tick(TickType type, Chunk chunk, Vector3I position) 
        {
            switch(type)
            {
                case TickType.World:
                    WorldTick(chunk, position);
                    break;

                case TickType.Block:
                    BlockTick(chunk, position);
                    break;
            }
        }

        protected virtual void WorldTick(Chunk chunk, Vector3I position) { }
        protected virtual void BlockTick(Chunk chunk, Vector3I position) { }

        public static void PlayerTickNeighbors(Vector3I globalBlockPosition)
        {
            throw new NotImplementedException("The player neighbors tick hasn't been implemented yet.");
        }

        public static void PlayerTickNeighbors(Chunk chunk, Vector3I position)
        {
            throw new NotImplementedException("The player neighbors tick hasn't been implemented yet.");
        }
    }
}
