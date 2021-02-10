using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace WEngine
{
    /// <summary>
    /// The delagate used for windows to invoke methods.
    /// </summary>
    public delegate void WindowInvokeDelegate();
    
    /// <summary>
    /// Implements a window.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// The surface size of the window when <see cref="Resizable"/> is true (in pixels), when the <see cref="WindowState"/> is Normal.
        /// </summary>
        public Vector2I SurfaceFixedResolution { get; set; }
        /// <summary>
        /// The surface size in pixels. Likely to change due to user resizing the window.
        /// </summary>
        public Vector2I SurfaceResolution { get; }
        /// <summary>
        /// Get the position of the surface in screen coordinates.
        /// </summary>
        public Vector2I SurfacePosition { get; set; }
        /// <summary>
        /// The screen state of the window.
        /// </summary>
        public WindowState WindowState { get; set; }
        /// <summary>
        /// The VSync mode of the window.
        /// </summary>
        public VSyncMode VSync { get; set; }
        /// <summary>
        /// With the user is allowed to grab the window
        /// </summary>
        public bool Resizable { get; set; }
        /// <summary>
        /// The thread used by the Window
        /// </summary>
        public Thread Thread { get; set; }
        /// <summary>
        /// If the window is focused.
        /// </summary>
        public bool Focused { get; }
        /// <summary>
        /// Is the cursor visible?
        /// </summary>
        public bool CursorVisible { get; set; }
        /// <summary>
        /// The text displayed into the title bar
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Event triggered when the window is resizing.
        /// </summary>
        public event EventHandler<EventArgs> OnResizing;

        /// <summary>
        /// Event triggered when the window has loaded.
        /// </summary>
        public event WindowInvokeDelegate OnLoaded;
        /// <summary>
        /// Event triggered when the window is updating.
        /// </summary>
        public event UpdateEventHandler OnUpdate;
        /// <summary>
        /// Event triggered when the window is rendering.
        /// </summary>
        public event UpdateEventHandler OnRender;

        /// <summary>
        /// Invoke an action on window's thread only once at update time.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        public void InvokeUpdate(Action action);
        /// <summary>
        /// Invoke an action on window's thread only once at render time.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        public void InvokeRender(Action action);
        /// <summary>
        /// Close the window.
        /// </summary>
        public void Close();
        /// <summary>
        /// Transforms a point from screen coordinates to window coordinates
        /// </summary>
        /// <param name="point">The screen point coordinates</param>
        /// <returns>The window space coordinates</returns>
        public Vector2I ScreenToWindow(Vector2I point);

        public Task WaitForNextFrame();
        public Bitmap Screenshot();
    }
}
