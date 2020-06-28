namespace Winecrash.Client
{
    /// <summary>
    /// The type of the ticket
    /// </summary>
    public enum TicketTypes
    {
        /// <summary>
        /// Ticket created by player
        /// </summary>
        Player,
        /// <summary>
        /// Start ticket created on world generation chunk
        /// </summary>
        Start,
        /// <summary>
        /// Used for world generation
        /// </summary>
        Temporary
    }
}
