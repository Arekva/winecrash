using System;

namespace Winecrash
{
    /// <summary>
    /// All the useful game folders.
    /// </summary>
    public static class Folders
    {
        /// <summary>
        /// The root game folder. Defaults to the folder containing the library.
        /// </summary>
        public static string Root { get; } = "/";
        /// <summary>
        /// The user data (saves, mods, settings) folder. Defaults to <c>Documents/Winecrash/</c>.
        /// </summary>
        public static string UserData { get; set; } = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Winecrash/";

    }
}
