using System;

namespace Winecrash
{
    public class Workbench : Cube, IContainer
    {
        public event ItemChangeDelegate OnItemAdd;

        public event ItemChangeDelegate OnItemRemove;

        public event ItemChangeDelegate OnItemUpdate;
        
        public override void SecondaryInteraction() => throw new NotImplementedException("Crafting is not implemented yet.");

        // no fast add : crafting table will display player inventory
        // and items must be moved manually in a certain slot.
        public void AddItemFast(ContainerItem item) {}

        public ContainerItem[] Items { get; set; } = new ContainerItem[10];
    }
}