using System.Runtime.InteropServices;

namespace WEngine
{
    /// <summary>
    /// Wrapper interface between system inputs and the engine.
    /// </summary>
    internal interface IInputWrapper
    {
        /// <summary>
        /// The corresponding Operating System to that wrapper.
        /// </summary>
        OSPlatform CorrespondingOS { get; }
        /// <summary>
        /// Get if a key is pressed.
        /// </summary>
        /// <param name="key">The wanted key.</param>
        /// <returns>If the key is pressed.</returns>
        bool GetKey(Keys key);
        /// <summary>
        /// Get the screen-space mouse cursor position.
        /// </summary>
        /// <returns>The screen-space position of the mouse/cursor.</returns>
        Vector2I GetMousePosition();
        /// <summary>
        /// Sets the screen-space mouse position.
        /// </summary>
        /// <param name="position">The new position of the mouse.</param>
        void SetMousePosition(Vector2I position);
    }
}
