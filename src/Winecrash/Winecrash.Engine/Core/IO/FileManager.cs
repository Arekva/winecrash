using System;
using System.Reflection;

namespace Winecrash.Engine
{
    public static class FileManager
    {
        public static string Root { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;
            
        [Initializer(Int32.MinValue)]
        private static void Initialize()
        {
            //CreateFolders();
        }

        private static void CreateFolders()
        {
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    if (!type.IsInterface && typeof(IFolder).IsAssignableFrom(type))
                    {
                        IFolder folder = Activator.CreateInstance(type) as IFolder;

                        System.IO.Directory.CreateDirectory(folder.Path);
                    }
                }
            }
        }
    }
}