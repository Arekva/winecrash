using WEngine;

namespace Winecrash
{
    public class ContainerItem : BaseObject
    {
        /// <summary>
        /// The actual item reference stored
        /// </summary>
        public Item Item { get; set; }
        
        //TODO: Use ulongs for AE-alike containers (servers :smirk:)
        /// <summary>
        /// The quantity of item into this slot
        /// </summary>
        public byte Amount { get; set; }

        public override void Delete()
        {
            Item = null;
            Amount = 0;
            
            base.Delete();
        }
    }
}