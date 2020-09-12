namespace WEngine
{
    public enum VSyncMode
    {
        /// <summary>
        /// Vsync disabled.
        /// </summary>
        Off = 0,

        /// <summary>
        /// VSync enabled.
        /// </summary>
        On = 1,

        /// <summary>
        /// VSync enabled, unless framerate falls below one half of target framerate. If
        /// <br>no target framerate is specified, this behaves exactly like VSyncMode.On.</br>
        /// </summary>
        Adaptive = 2
    }
}
