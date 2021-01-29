using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;


namespace Winecrash.Installer
{
    public static class Utilities
    {
        public static string CustomPathFileName { get; set; } = "CustomPath";
        public static string LauncherExecutable { get; set; } = "Winecrash.Launcher.exe";
        public static string DefaultInstallDirectory => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        
        public static string CustomPathFilePath => Path.Combine(DefaultInstallDirectory, ApplicationName, CustomPathFileName);

        public static string InstallDirectory => File.Exists(CustomPathFilePath) ? File.ReadAllText(CustomPathFilePath) : DefaultInstallDirectory;
        public static string LauncherFolder { get; set; } = "Launcher";
        public static string ProfilesFolder { get; set; } = "Profiles";
        public static string PackagesFolder { get; set; } = "Packages";
        public static string ApplicationName { get; set; } = "Winecrash";

        public static string ApplicationPath => Path.Combine(InstallDirectory, ApplicationName);
        public static string LauncherPath => Path.Combine(ApplicationPath, LauncherFolder);
        public static string ProfilesPath => Path.Combine(ApplicationPath, ProfilesFolder);
        public static string PackagesPath => Path.Combine(ApplicationPath, PackagesFolder);
        public static string LauncherExecutablePath => Path.Combine(LauncherPath, LauncherExecutable);

        public static bool LauncherInstalled => Directory.Exists(LauncherPath);
        public static string ZipPath => Path.Combine(Utilities.ApplicationPath, "temp.zip");

        public static string DownloadLink { get; set; } = @"http://docs.arekva.fr/f/uzOCht/launcher.zip";

        public static bool InstalledAtPath(string path) => Directory.Exists(Path.Combine(path, LauncherFolder));
        

        public static void CreateCustomPathFile(string path)
        {
            Directory.CreateDirectory(ApplicationPath);
            File.WriteAllText(CustomPathFilePath, path);
        }

        public static void SetupForInstallation(string path)
        {
            if (path != InstallDirectory) CreateCustomPathFile(path);

            Directory.CreateDirectory(LauncherPath);
            Directory.CreateDirectory(ProfilesPath);
            Directory.CreateDirectory(PackagesPath);
        }

        public static void CreateShortcutDesktop(string to) => CreateShortcut(to, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ApplicationName));

        public static void CreateShortcutStart(string to) => CreateShortcut(to, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", ApplicationName));
        public static void CreateShortcut(string to, string path) => CreateLink(to, path, null);

        public static void CreateLink(string srcPath, string shortcutPath, string description = null)
        {
            // Derive params
            if (description == null)
                description = "Opens " + Path.GetFileNameWithoutExtension(srcPath);
            if (!shortcutPath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                shortcutPath += ".lnk";

            // Initialize Shortcut
            var link = (IShellLink)new ShellLink();
            link.SetDescription(description);
            link.SetPath(srcPath);
            link.SetWorkingDirectory(Path.GetDirectoryName(srcPath));

            // Save
            var pf = (IPersistFile)link;
            pf.Save(shortcutPath, false);
        }
        

            // Load all suffixes in an array  
            private static readonly string[] sizeSuffixes =  
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };

            public static string FormatSize(Int64 bytes)
            {
                int counter = 0;
                decimal number = (decimal) bytes;
                while (Math.Round(number / 1024) >= 1)
                {
                    number = number / 1024;
                    counter++;
                }

                return $"{number:n1}{sizeSuffixes[counter]}";
            }
        
    }
}