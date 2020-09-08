using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Game
{
    public delegate void LanguageChangeDelegate(Language language);
    public static class Game
    {
        public static event LanguageChangeDelegate OnLanguageChanged; 
        public static GameConfig Configuration { get; } = new GameConfig("settings.json");

        /// <summary>
        /// The version of the game. Read only.
        /// </summary>
        public static Version Version { get; set; } = new Version(0, 0, 1, "Alpha");

        static Game()
        {
            LoadLanguagesInFolder("assets/languages/");
        }

        internal static void InvokeLanguageChanged(Language lang)
        {
            OnLanguageChanged?.Invoke(lang);
        }

        internal static List<Language> LoadLanguagesInFolder(string path)
        {
            List<Language> langs = new List<Language>();
            foreach(string str in Directory.GetFiles(path, "*.json"))
            {
                try
                {
                    langs.Add(new Language(str));
                }
                catch(Exception e) { }
            }

            return langs;
        }
    }
}
