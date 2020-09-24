namespace Winecrash
{
    public enum TickType
    {
        /// <summary>
        /// World, random world tick, mostly used to update grass, crops...
        /// </summary>
        World,
        /// <summary>
        /// Block tick, used when a player updates a neighbor block
        /// </summary>
        Block
    }
}