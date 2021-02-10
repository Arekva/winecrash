namespace Winecrash
{
    public delegate void ItemChangeDelegate(ContainerItem item, int slot);
    public interface IContainer
    {
        event ItemChangeDelegate OnItemAdd;

        event ItemChangeDelegate OnItemRemove;

        event ItemChangeDelegate OnItemUpdate;
        
        ContainerItem[] Items { get; set; }
        
        int[] ReadonlyIndices { get; set; }

        /// <summary>
        /// Adds the item in the first slot available.
        /// </summary>
        /// <param name="item">The item to add. Some amount of item might not be added so try to manage to resulting ContainerItem.</param>
        void AddItemFast(ContainerItem item);
    }
}