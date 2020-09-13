namespace WEngine
{
    /// <summary>
    /// All the cursor lock modes.
    /// </summary>
    public enum CursorLockModes
    {
        /// <summary>
        /// The cursor is free and the user can move the cursor all around.
        /// </summary>
        Free = 1,
        /// <summary>
        /// The cursor is locked at screen centre, ideal for an FPS controler.
        /// </summary>
        Lock = 2,
        /// <summary>
        /// [NOT IMPLEMENTED YET] The cursor is confined inside the screen, but free to move into it.
        /// </summary>
        Confined = 3
    }
}
