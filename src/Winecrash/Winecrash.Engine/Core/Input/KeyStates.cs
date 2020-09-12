namespace WEngine
{
    /// <summary>
    /// All the possible states of a key.
    /// </summary>
    [System.Flags]
    internal enum KeyStates : byte
    {
        /// <summary>
        /// If no state registered. Only should happen on <see cref="Input.RegisteredKeyStates"/> instatiation.
        /// </summary>
        None = 0,
        /// <summary>
        /// Idle state; the key is entirely released.
        /// </summary>
        Released = 1,
        /// <summary>
        /// Pressed state; the user is actively pressing it.
        /// </summary>
        Pressed = 2,
        /// <summary>
        /// Releasing state; the key is in a releasing process: previously was <see cref="Pressed"/> and will be <see cref="Released"/>.
        /// </summary>
        Releasing = 4,
        /// <summary>
        /// Pressing state; the key is in a pressing process: previously was <see cref="Released"/> and will be <see cref="Pressed"/>.
        /// </summary>
        Pressing = 8
    }
}
