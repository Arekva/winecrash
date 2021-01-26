using System;
using System.IO;

namespace Winecrash.Installer
{
    public static class Utilities
    {
        public static string CustomPathFileName { get; set; } = "CustomPath";
        public static string EmbeddedFileName { get; set; } = "Winecrash.Installer.Properties.Resources.resources";
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

        public static bool LauncherInstalled => Directory.Exists(LauncherPath);

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
    }
}