using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;
using System.Configuration;

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
                return Folders.UserData + "Saves/";
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

                this.Path = System.IO.Path.Combine(FolderPath, fileName);

                if (Directory.Exists(Path))
                {
                    this.Informations = new SaveInfoConfig(System.IO.Path.Combine(Path, SaveInfoConfig.RelativePath));
                }

                else
                {
                    throw new SaveException($"The save \"{name}\" does not exist.");
                }
            }
            else
            {
                this.Path = System.IO.Path.Combine(FolderPath, fileName);

                int count = 0;
                while (Directory.Exists(this.Path))
                {
                    this.Path += $" ({++count})";
                }

                Directory.CreateDirectory(this.Path);

                Informations = new SaveInfoConfig(System.IO.Path.Combine(this.Path, SaveInfoConfig.RelativePath))
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

        /// <summary>
        /// Get all the saves in <see cref="FolderPath"/> with their status
        /// </summary>
        /// <returns>All the saves within the folder by the format: [ <see cref="string"/> folder , [ <see cref="Save"/> save, <see cref="SaveStatus"/> status ] ].
        /// <br>If unknown or corrupted, the <see cref="Save"/> might be null.</br></returns>
        public static KeyValuePair<string, KeyValuePair<Save, SaveStatus>>[] GetAllSaves()
        {
            if(!Directory.Exists(FolderPath))
            {
                return Array.Empty<KeyValuePair<string, KeyValuePair<Save, SaveStatus>>>();
            }
            string[] directories = Directory.GetDirectories(FolderPath);

            KeyValuePair<string, KeyValuePair<Save, SaveStatus>>[] saves = new KeyValuePair<string, KeyValuePair<Save, SaveStatus>>[directories.Length];

            string fileName;
            SaveStatus status;
            Save save;
            for (int i = 0; i < directories.Length; i++)
            {
                fileName = directories[i].Split('/', '\\').Last();
                status = CheckIntegrity(directories[i], out save);
                saves[i] = new KeyValuePair<string, KeyValuePair<Save, SaveStatus>>(fileName, new KeyValuePair<Save, SaveStatus>(save, status));
            }

            return saves;
        }

        public static SaveStatus CheckIntegrity(string path, out Save save)
        {
            SaveStatus status = File.Exists(System.IO.Path.Combine(path, SaveInfoConfig.RelativePath)) ? SaveStatus.Created : SaveStatus.Unknown;

            if (File.Exists(System.IO.Path.Combine(path, SaveInfoConfig.RelativePath)))
            {
                try
                {
                    save = new Save(path.Split('\\', '/').Last(), true);
                    status |= SaveStatus.Sane;

                    if(save.Informations.Version < Game.Version)
                    {
                        status |= SaveStatus.OutOfDate;
                    }
                }
                catch (Exception e)
                {
                    save = null;
                    status |= SaveStatus.Corrupted;
                }
            }

            else
            {
                save = null;
                status |= SaveStatus.Corrupted;
            }
            return status;
        }
    }
}
