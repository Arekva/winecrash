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
        /// <summary>
        /// Triggered when a new game language is set. Carries the new language object with it.
        /// </summary>
        public static event LanguageChangeDelegate OnLanguageChanged;


        internal static GameConfig Configuration { get; } = new GameConfig("settings.json");

        /// <summary>
        /// The version of the game. Read only.
        /// </summary>
        public static Version Version { get; set; } = new Version(0, 0, 1, "Alpha");

        public const string DefaultLanguage = "English (United Kingdom)";

        /// <summary>
        /// The language used by the game. Defaults to <see cref="DefaultLanguage"/>
        /// </summary>
        public static Language Language
        {
            get
            {
                return Winecrash.Game.Language.Languages.FirstOrDefault(l => l.Name == Configuration.Language);
            }

            set
            {
                if (value == null)
                {
                    Configuration.Language = DefaultLanguage;
                }

                else
                {
                    Configuration.Language = value.Name;
                }

                Game.InvokeLanguageChanged(value);
            }
        }

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
                catch (Exception)
                {
                    continue;
                }
            }

            return langs;
        }
    }
}
