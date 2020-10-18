using System;
using System.Reflection;

namespace WEngine
{
    /// <summary>
    /// The engine file mananger. Has to be redone.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// The root path of the application.
        /// </summary>
        public static string Root { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;
            
        //[Initializer(Int32.MinValue)]
        private static void Initialize()
        {
            //CreateFolders();
        }

        /// <summary>
        /// Create all the folders (searches for classes implementing <see cref="IFolder"/>).
        /// </summary>
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