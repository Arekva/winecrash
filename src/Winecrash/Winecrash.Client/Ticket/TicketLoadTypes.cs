namespace Winecrash.Client
{
    /// <summary>
    /// The load type of the ticket
    /// </summary>
    public enum TicketLoadTypes
    {
        /// <summary>
        /// All the ticks are available
        /// </summary>
        EntityTicking,
        /// <summary>
        /// Blocks only are ticking
        /// </summary>
        Ticking,
        /// <summary>
        /// Between ticking and generation only blocks
        /// </summary>
        Border,
        /// <summary>
        /// World generation only
        /// </summary>
        Inaccessible,
    }
}
