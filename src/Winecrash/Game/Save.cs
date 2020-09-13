using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;

namespace Winecrash
{
    public class Save : BaseObject
    {
        /// <summary>
        /// The default save name.
        /// </summary>
        public static string DefaultName { get; } = "Default";

        /// <summary>
        /// The save folder path. <see cref="Folders.UserData"/> to set global user data.
        /// </summary>
        public static string FolderPath
        {
            get
            {
                return Folders.UserData + "/Saves/";
            }
        }

        /// <summary>
        /// Path of this save folder relative to <see cref="Folders.UserData"/>.
        /// </summary>
        public string Path { get; set; }

        public SaveInfoConfig Informations { get; }

        /// <summary>
        /// Create or load a save by a name. If a save folder already exist with this name, it adds a number to it.
        /// </summary>
        /// <param name="name">The name of the save folder. If no name provided, <see cref="DefaultName"/> will be taken. If already existing, adds a counter.</param>
        /// <param name="create">Do the save has to be loaded if existing.</param>
        public Save(string name, bool load) : base(name)
        {
            if (String.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                name = this.Name = DefaultName;
            }

            string fileName = ToValidFileName(name);

            if (load)
            {
                this.Name = name;

                this.Path = FolderPath + fileName + "/";

                if (Directory.Exists(Path))
                {
                    this.Informations = new SaveInfoConfig(Path + "/header.json");
                }

                else
                {
                    throw new SaveException($"The save \"{name}\" does not exist ! ");
                }
            }
            else
            {
                this.Path = FolderPath + fileName + "/";

                int count = 0;
                while (Directory.Exists(this.Path))
                {
                    this.Path = FolderPath + fileName + $" ({++count})/";
                }

                Directory.CreateDirectory(this.Path);

                Informations = new SaveInfoConfig(this.Path + "header.json")
                {
                    Name = name
                };
            }
        }

        /// <summary>
        /// Makes a file name valid by changing unvalid characters.
        /// </summary>
        /// <param name="name">The file name.</param>
        /// <param name="replacement">The invalid replacement string.</param>
        /// <returns>A valid file name.</returns>
        public static string ToValidFileName(string name, string replacement = "-")
        {
            return name.Replace(System.IO.Path.GetInvalidFileNameChars(), replacement);
        }
    }
}
